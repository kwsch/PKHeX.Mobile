using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using AppInfo = PKHeX.Mobile.Logic.AppInfo;

namespace PKHeX.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About PKHeX Mobile";

            OpenWebCommand = new Command(async () => await Launcher.OpenAsync(AppInfo.GitHubLink).ConfigureAwait(false));
        }

        public ICommand OpenWebCommand { get; }
    }
}