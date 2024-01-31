using PKHeX.Core;
using PKHeX.Mobile.Logic;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

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
            AppCenter.Start("ios=6696eefb-f004-4303-963d-72a3fe9a8061;" +
                  "uwp={Your UWP App secret here};" +
                  "android={Your Android App secret here};" +
                  "macos={Your macOS App secret here};",
                  typeof(Analytics), typeof(Crashes));
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
