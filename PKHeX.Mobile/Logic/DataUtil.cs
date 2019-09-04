using System.Threading.Tasks;
using Acr.UserDialogs;
using Xamarin.Essentials;

namespace PKHeX.Mobile.Logic
{
    public static class DataUtil
    {
        public static async Task<string> GetClipboardText()
        {
            if (!Clipboard.HasText)
            {
                await UserDialogs.Instance
                    .AlertAsync("No data in device Clipboard!")
                    .ConfigureAwait(false);
                return null;
            }
            try
            {
                var txt = await Clipboard
                    .GetTextAsync()
                    .ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(txt))
                {
                    await UserDialogs.Instance
                        .AlertAsync("The data in the clipboard does not contain data!")
                        .ConfigureAwait(false);
                    return null;
                }

                return txt;
            }
            catch
            {
                await UserDialogs.Instance
                    .AlertAsync("No data in device Clipboard!")
                    .ConfigureAwait(false);
                return null;
            }
        }
    }
}