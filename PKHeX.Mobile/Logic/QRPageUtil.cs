using System;
using System.Threading.Tasks;

using Acr.UserDialogs;
using PKHeX.Core;
using Xamarin.Forms;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace PKHeX.Mobile.Logic
{
    public static class QRPageUtil
    {
        public static async Task<PKM> ScanQRPKM(SaveFile sav, INavigation nav)
        {
            var scanPage = new ZXingScannerPage
            {
                DefaultOverlayTopText = "Hold camera up to QR code",
                DefaultOverlayBottomText = "Camera will automatically scan QR code/Barcode \r\n\rPress the 'Back' button to cancel"
            };
            scanPage.AutoFocus();

            PKM pkm = null;
            bool finished = false;
            scanPage.OnScanResult += async result =>
            {
                // disable scanning until message is acknowledged; if we get a good result, we're done scanning anyway
                scanPage.IsScanning = false;
                if (result.BarcodeFormat != BarcodeFormat.QR_CODE)
                {
                    await UserDialogs.Instance.AlertAsync("That's not a QR code.").ConfigureAwait(false);
                    scanPage.IsScanning = true;
                    return;
                }

                var pk = QRUtil.GetPKMFromQRMessage(result.Text, sav.Generation);
                if (pk == null)
                {
                    await UserDialogs.Instance.AlertAsync("Please scan a PKM QR code.").ConfigureAwait(false);
                    scanPage.IsScanning = true;
                    return;
                }

                pkm = PKMConverter.ConvertToType(pk, sav.PKMType, out var c);
                if (pkm == null)
                {
                    Console.WriteLine(c);
                    await UserDialogs.Instance.AlertAsync("Please scan a compatible PKM format QR code." + Environment.NewLine + $"Received {pk.GetType().Name}").ConfigureAwait(false);
                    scanPage.IsScanning = true;
                    return;
                }

                finished = true;
            };
            scanPage.Disappearing += (s, e) => finished = true;

            await nav.PushAsync(scanPage).ConfigureAwait(false);
            while (!finished)
                await Task.Delay(100).ConfigureAwait(false);
            if (pkm == null) // canceled
                return null;
            Device.BeginInvokeOnMainThread(async () => await nav.PopAsync().ConfigureAwait(false));
            return pkm;
        }
    }
}
