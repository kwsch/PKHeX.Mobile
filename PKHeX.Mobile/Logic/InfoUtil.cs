using System.IO;
using PKHeX.Core;

namespace PKHeX.Mobile.Logic
{
    public static class InfoUtil
    {
        public static string GetProgramTitle(SaveFile sav)
        {
            string title = $"{sav.GetType().Name}: ";
            if (!sav.State.Exportable) // Blank save file
            {
                var ver = GameInfo.GetVersionName(sav.Version);
                return title + $" [{sav.OT} ({ver})]";
            }

            var bakname = sav.Metadata.BAKName;
            bakname = bakname.Substring(bakname.IndexOf('[')); // trim off filename
            return title + Path.GetFileNameWithoutExtension(bakname); // trim off ext
        }
    }
}