using System;
using PKHeX.Core;
using PKHeX.Drawing;
using SkiaSharp.Views.Forms;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PKHeX.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRView : ContentPage
    {
        public QRView(PKM pkm)
        {
            InitializeComponent();

            Header.Text = "PKHeX Mobile!" + Environment.NewLine + $"Data Format: {pkm.GetType().Name}";
            Preview.Source = (SKBitmapImageSource)pkm.Sprite();

            // get high resolution QR with dimensions best suited to the device's dimensions
            // we don't overlay the pkm sprite in this QR since we do it above
            var di = DeviceDisplay.MainDisplayInfo;
            var width = Math.Min(di.Height, di.Width);
            var img = QRBuilder.GetQR(pkm, (int)(width * 0.8));
            QR.Source = (SKBitmapImageSource)img;

            Desc.Text = ShowdownSet.GetShowdownText(pkm);
        }
    }
}