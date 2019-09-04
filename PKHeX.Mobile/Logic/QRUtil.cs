using System;
using System.Linq;
using PKHeX.Core;

namespace PKHeX.Mobile.Logic
{
    /// <summary>
    /// QR Message reading &amp; writing logic
    /// </summary>
    public static class QRUtil
    {
        private const string QR6PathBad = "null/#";
        private const string QR6Path = "http://lunarcookies.github.io/b1s1.html#";
        public static string GetQRServer(int format) => format == 6 ? QR6Path : QR6PathBad;

        public static PKM GetPKMFromQRMessage(string message, int format)
        {
            var pkdata = GetPKMDataFromQRResult(message);
            if (pkdata == null)
                return null;
            return PKMConverter.GetPKMfromBytes(pkdata, format);
        }

        public static string GetQRMessage(PKM pkm)
        {
            if (pkm is PK7 pk7)
            {
                byte[] payload = QR7.GenerateQRData(pk7);
                return string.Concat(payload.Select(z => (char)z));
            }

            var server = GetQRServer(pkm.Format);
            var data = pkm.EncryptedBoxData;
            string qrdata = Convert.ToBase64String(data);
            return server + qrdata;
        }

        private static byte[] GetPKMDataFromQRResult(string message)
        {
            if (message.Length < 32) // arbitrary length check; everything should be greater than this
                return null;
            if (message.StartsWith(QR6PathBad)) // fake url
                return GetDataFromURLQR(message);
            if (message.StartsWith("http")) // inject url
                return GetDataFromURLQR(message);
            if (message.StartsWith("POKE") && message.Length > 0x30 + 0xE8) // G7 data
                return GetBytesFromMessage(message, 0x30, 0xE8);
            return null;
        }

        private static byte[] GetDataFromURLQR(string url)
        {
            try
            {
                int payloadBegin = url.IndexOf('#');
                if (payloadBegin < 0) // bad URL, need the payload separator
                    return null;
                url = url.Substring(payloadBegin + 1); // Trim URL to right after #
                return Convert.FromBase64String(url);
            }
            catch
            {
                return null;
            }
        }

        private static byte[] GetBytesFromMessage(string seed, int skip, int take)
        {
            byte[] data = new byte[take];
            for (int i = 0; i < take; i++)
                data[i] = (byte)seed[i + skip];
            return data;
        }
    }
}