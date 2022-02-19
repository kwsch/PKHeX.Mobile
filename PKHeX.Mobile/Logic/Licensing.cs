using System;
using System.Text;
using Syncfusion.Licensing;

namespace PKHeX.Mobile.Logic
{
    // just silly obfuscation, better than being plaintext
    // anyone dedicated enough can just peek in the apk to get keys if things were completely hidden from source control... be responsible!
    public static class Licensing
    {
        public static void LoadLicenses()
        {
            var type = typeof(SyncfusionLicenseProvider);
            var methods = type.GetMethods();
            var m = methods[0];
            m.Invoke(null, new object[] { Decode(NothingImportant) });
        }

        private const string NothingImportant = "TlRnME5ESXlRRE14TXpreVpUTTBNbVV6TUdaNWRTOUpNbXBIZG5KRVptTkxXbXBKYm5VdlJUWnJWV3A2VG1KRU1tMWhlakZ2TXpkalpWaDBXRkU5";
        private static string Decode(string str) => Encoding.ASCII.GetString(Convert.FromBase64String(str));
        private static string Encode(string str) => Convert.ToBase64String(Encoding.ASCII.GetBytes(str));
    }
}
