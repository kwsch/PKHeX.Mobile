using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using PKHeX.Core;

using Acr.UserDialogs;
using Xamarin.Essentials;

namespace PKHeX.Mobile.Logic
{
    // todo: rename this class to not clash with PKHeX.Core
    public static class FileUtil
    {
        private static string outputFolder = "/storage/emulated/0/PkHex/";
        public static async Task<FileResult> PickFile()
        {
            var fileData = await FilePicker.PickAsync(PickOptions.Default).ConfigureAwait(false);
            if (fileData == null)
                return null; // user canceled file picking
            Debug.WriteLine($"File name chosen: {fileData.FileName}");
            Debug.WriteLine($"File path chosen: {fileData.FullPath}");
            return fileData;
        }

        public static async Task<SaveFile> TryGetSaveFile()
        {
            try
            {
                var file = await PickFile().ConfigureAwait(false);
                if (file == null)
                    return null;

                await using var stream = await file.OpenReadAsync().ConfigureAwait(false);
                var len = stream.Length;
                bool isPossibleSAV = SaveUtil.IsSizeValid((int) len);
                if (!isPossibleSAV)
                    return null;

                var data = new byte[len];
                stream.Read(data);
                var sav = SaveUtil.GetVariantSAV(data);
                sav?.Metadata.SetExtraInfo(file.FullPath);
                return sav;
            }
            catch (FileNotFoundException ex)
            {
                await UserDialogs.Instance.AlertAsync($"The file is being passed as a URI instead of a path. Please try moving your saves to a different folder.\n\nStack Trace:\n{ex}").ConfigureAwait(false);
                return null;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                await UserDialogs.Instance.AlertAsync($"Exception choosing file: {ex}").ConfigureAwait(false);
                return null;
            }
        }

        public static SaveFile TryGetSaveFile(string filePath)
        {
            try
            {
                var data = File.ReadAllBytes(filePath);
                var len = data.Length;
                bool isPossibleSAV = SaveUtil.IsSizeValid(len);
                if (!isPossibleSAV)
                    return null;
                var sav = SaveUtil.GetVariantSAV(data);
                sav?.Metadata.SetExtraInfo(filePath);
                return sav;
            }
            catch
            {
                //Ignore errors as this is meant to be a background scanning function
                return null;
            }
        }

        public static async Task<bool> ExportSAV(SaveFile sav)
        {
            if (!sav.State.Exportable)
            {
                await UserDialogs.Instance.AlertAsync("Can't export the current save file.").ConfigureAwait(false);
                return false;
            }

            //Create directory structure
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            String myDate = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            if (!Directory.Exists(outputFolder + myDate + "/"))
            {
                Directory.CreateDirectory(outputFolder + myDate + "/");
            }

            var data = sav.Write();
            var path = outputFolder + myDate + "/" + Path.GetFileName(sav.Metadata.FilePath);
            sav?.Metadata.SetExtraInfo(path);
            Debug.WriteLine($"File path moved: {sav.Metadata.FilePath}");
            try
            {
                if (sav.State.Exportable)
                    await SaveBackup(sav).ConfigureAwait(false);
                File.Create(path).Close();
                await File.WriteAllBytesAsync(path, data).ConfigureAwait(false);
                await UserDialogs.Instance.AlertAsync($"Exported save file to: {path}").ConfigureAwait(false);
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                await UserDialogs.Instance.AlertAsync($"Failed to access \"" + outputFolder + "\" please grant All File Access Special Permision").ConfigureAwait(false);
                //await UserDialogs.Instance.AlertAsync($"Failed: {ex}").ConfigureAwait(false);
                return false;
            }
        }

        private static async Task SaveBackup(SaveFile sav)
        {
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var bakPath = Path.Combine(docPath, "PKHeX Backups");
            Directory.CreateDirectory(bakPath);

            if (!Directory.Exists(bakPath))
            {
                Console.WriteLine("Backup path does not exist.");
                return;
            }

            var bakName = Path.Combine(bakPath, Util.CleanFileName(sav.Metadata.BAKName));
            if (File.Exists(bakName))
            {
                Console.WriteLine("Backup already exists for this file.");
                return;
            }

            await File.WriteAllBytesAsync(bakName, sav.State.BAK).ConfigureAwait(false);
            bool success = File.Exists(bakName);
            Console.WriteLine($"Backed up: {success}");
        }
    }
}
