using System;
using System.Windows.Input;
using PKHeX.Mobile.Logic;
using Xamarin.Forms;

namespace PKHeX.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About PKHeX Mobile";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri(AppInfo.GitHubLink)));
        }

        public ICommand OpenWebCommand { get; }
    }
}