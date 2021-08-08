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
        public static async Task<FileResult> PickFile()
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
                if (sav.State.Exportable)
                    await SaveBackup(sav).ConfigureAwait(false);

                await File.WriteAllBytesAsync(path, data).ConfigureAwait(false);
                await UserDialogs.Instance.AlertAsync($"Exported save file to: {path}").ConfigureAwait(false);
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                await UserDialogs.Instance.AlertAsync($"Failed: {ex}").ConfigureAwait(false);
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
