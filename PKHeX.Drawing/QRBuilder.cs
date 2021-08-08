using System;
using System.Linq;
using PKHeX.Core;
using SkiaSharp;
using SkiaSharp.QrCode;

namespace PKHeX.Drawing
{
    public static class QRBuilder
    {
        private const string QR6PathBad = "null/#";
        private const string QR6Path = "http://lunarcookies.github.io/b1s1.html#";

        public static string GetQRServer(int format)
        {
            return format == 6 ? QR6Path : QR6PathBad;
        }

        public static string GetQRMessage(PKM pkm)
        {
            if (pkm is PK7 pk7)
            {
                byte[] payload = QR7.GenerateQRData(pk7);
                return string.Concat(payload.Select(z => (char) z));
            }

            var server = GetQRServer(pkm.Format);
            var data = pkm.EncryptedBoxData;
            string qrdata = Convert.ToBase64String(data);
            return server + qrdata;
        }

        public static SKBitmap GetQR(PKM pkm, int dim = 365)
        {
            string content = GetQRMessage(pkm);
            using var generator = new QRCodeGenerator();

            // Generate QrCode
            var qr = generator.CreateQrCode(content, ECCLevel.H);

            // Render to canvas
            var info = new SKImageInfo(dim, dim);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            canvas.Render(qr, info.Width, info.Height);

            return SKBitmap.Decode(surface.Snapshot().Encode().AsStream());
        }
    }
}
