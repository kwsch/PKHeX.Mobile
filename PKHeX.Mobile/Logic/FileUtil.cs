using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using PKHeX.Core;

using Acr.UserDialogs;
using Plugin.FilePicker;

namespace PKHeX.Mobile.Logic
{
    // todo: rename this class to not clash with PKHeX.Core
    public static class FileUtil
    {
        public static async Task<string> PickFile(params string[] ext)
        {
            using (var fileData = await CrossFilePicker.Current.PickFile(ext).ConfigureAwait(false))
            {
                if (fileData == null)
                    return null; // user canceled file picking
                Debug.WriteLine($"File name chosen: {fileData.FileName}");
                return fileData.FilePath;
            }
        }

        public static async Task<SaveFile> TryGetSaveFile()
        {
            try
            {
                var path = await PickFile().ConfigureAwait(false);
                if (path == null)
                    return null;

                var fi = new FileInfo(path);
                var len = fi.Length;
                bool isPossibleSAV = SaveUtil.IsSizeValid((int) len);
                if (!isPossibleSAV)
                    return null;

                var data = File.ReadAllBytes(path);
                var sav = SaveUtil.GetVariantSAV(data);
                sav?.SetFileInfo(path);
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
            if (!sav.Exportable)
            {
                await UserDialogs.Instance.AlertAsync("Can't export the current save file.").ConfigureAwait(false);
                return false;
            }

            var data = sav.Write();
            var path = sav.FilePath;
            try
            {
                var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var bakPath = Path.Combine(docPath, "PKHeX Backups");
                Directory.CreateDirectory(bakPath);

                var bakName = Path.Combine(bakPath, Util.CleanFileName(sav.BAKName));
                if (sav.Exportable && Directory.Exists(bakPath) && !File.Exists(bakName))
                    File.WriteAllBytes(bakName, sav.BAK);
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
                return fat.HasFlag(FileAttributes.ReadOnly);
            }
            catch { return true; }
        }
    }
}