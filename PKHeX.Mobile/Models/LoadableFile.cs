using PKHeX.Core;
using PKHeX.Drawing;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace PKHeX.Mobile.Models
{
    public class LoadableSAV
    {
        public string Name => System.IO.Path.GetFileName(File.FilePath);
        public string Info => File.FileFolder;
        public string TrainerName => File.OT;
        public string PlayTime => File.PlayTimeString;
        public string Game => GameInfo.GetVersionName(File.Version);

        public ImageSource ImageUrl
        {
            get
            {
                var ver = (GameVersion)File.Game;
                var gsprite = SpriteBuilder.GetGameIcon(ver) ?? SpriteBuilder.GetGameIcon(File.Version);
                return (SKBitmapImageSource)gsprite;
            }
        }

        public string Path { get; }
        public SaveFile File { get; }

        public LoadableSAV(string path, SaveFile file)
        {
            Path = path;
            File = file;
        }

        public bool LocatedAt(string path) => path == Path;
    }
}
