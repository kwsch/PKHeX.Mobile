using PKHeX.Core;
using PKHeX.Mobile.Logic;
using Xamarin.Forms;

namespace PKHeX
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            Licensing.LoadLicenses();
            GameInfo.Strings = GameInfo.GetStrings(GameInfo.CurrentLanguage);
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
