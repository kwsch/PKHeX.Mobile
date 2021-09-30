using System;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using PKHeX.Core;
using PKHeX.Core.AutoMod;
using PKHeX.Mobile.Models;
using PKHeX.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;

namespace PKHeX.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Loader : ContentPage
    {
        private static string outputFolder = "/storage/emulated/0/PkHex/";
        public Loader()
        {
            InitializeComponent();
            initialLoad();
        }

        private async void initialLoad()
        {
            try
            {
                if (CV_Saves.SelectedItem == null)
                {
                    if (Directory.Exists(outputFolder))
                    {
                        foreach (string filePath in Directory.EnumerateFiles(outputFolder, "*", SearchOption.AllDirectories))
                        {
                            var sav = Logic.FileUtil.TryGetSaveFile(filePath);
                            if (sav != null)
                            {
                                var match = VM.Saves.FirstOrDefault(z => z.LocatedAt(sav.Metadata.FilePath));
                                if (match != null)
                                    CV_Saves.SelectedItem = match;
                                else
                                    await LoadNewSaveFile(sav).ConfigureAwait(false);
                            }
                        }
                    }
                }
            } catch
            {
                //first time load, ignore error
                return;
            }
        }

        private async void TgrOpenTapped(object sender, EventArgs e)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            await Open((LoadableSAV)CV_Saves.SelectedItem).ConfigureAwait(false);
            IsBusy = false;
        }

        private async void TgrCacheTapped(object sender, EventArgs e)
        {
            if (IsBusy)
                return;
            IsBusy = true;

            var sav = await Logic.FileUtil.TryGetSaveFile().ConfigureAwait(false);
            if (sav == null)
            {
                IsBusy = false;
                return;
            }

            var match = VM.Saves.FirstOrDefault(z => z.LocatedAt(sav.Metadata.FilePath));
            if (match != null)
                CV_Saves.SelectedItem = match;
            else
                await LoadNewSaveFile(sav).ConfigureAwait(false);
            IsBusy = false;
        }

        private async Task LoadNewSaveFile(SaveFile sav)
        {
            if (sav == null)
            {
                await UserDialogs.Instance.AlertAsync("No Save File selected.").ConfigureAwait(false);
                return;
            }

            if (!sav.ChecksumsValid)
            {
                const string proceed = "Continue";
                var result = await UserDialogs.Instance.ActionSheetAsync("Bad checksums detected.", "Abort", null, null, proceed).ConfigureAwait(false);
                if (result != proceed)
                    return;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                var l = new LoadableSAV(sav.Metadata.FilePath, sav);
                TrainerSettings.Register(sav);
                VM.Saves.Add(l);
                CV_Saves.SelectedItem = l;
                B_Open.IsVisible = true;
            });
        }

        private async Task Open(LoadableSAV l)
        {
            var sav = l.File;
            sav.State.Edited = true;
            var ed = new SaveEditor();
            var binding = (SaveFileViewModel)ed.BindingContext;
            binding.Initialize(sav);
            await Navigation.PushAsync(page: ed, true).ConfigureAwait(false);
        }
    }
}