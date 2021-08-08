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
        public static async Task<FileResult> PickFile(params string[] ext)
        {
            var fileData = await FilePicker.PickAsync(PickOptions.Default).ConfigureAwait(false);
            if (fileData == null)
                return null; // user canceled file picking
            Debug.WriteLine($"File name chosen: {fileData.FileName}");
            return fileData;
        }

        public static async Task<SaveFile> TryGetSaveFile()
        {
            try
            {
                var file = await PickFile().ConfigureAwait(false);
                if (file == null)
                    return null;

                await using var stream = await file.OpenReadAsync();
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
            catch (Exception ex)
            {
                await UserDialogs.Instance.AlertAsync($"Exception choosing file: {ex}").ConfigureAwait(false);
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

            var data = sav.Write();
            var path = sav.Metadata.FilePath;
            try
            {
                var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var bakPath = Path.Combine(docPath, "PKHeX Backups");
                Directory.CreateDirectory(bakPath);

                var bakName = Path.Combine(bakPath, Util.CleanFileName(sav.Metadata.BAKName));
                if (sav.State.Exportable && Directory.Exists(bakPath) && !File.Exists(bakName))
                    File.WriteAllBytes(bakName, sav.State.BAK);
                bool success = File.Exists(bakName);
                Console.WriteLine($"Backed up: {success}");

                File.WriteAllBytes(path, data);
                await UserDialogs.Instance.AlertAsync("Exported save file!").ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                await UserDialogs.Instance.AlertAsync($"Failed: {ex}").ConfigureAwait(false);
                return false;
            }
        }

        private static bool IsFileLocked(string path)
        {
            try
            {
                var fat = File.GetAttributes(path);
                return (fat & FileAttributes.ReadOnly) != 0;
            }
            catch { return true; }
        }
    }
}