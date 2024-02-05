using System;
using System.ComponentModel;
using System.Linq;
using PKHeX.Mobile.Logic;
using PKHeX.ViewModels;
using Xamarin.Forms;
using SelectionChangedEventArgs = Syncfusion.XForms.ComboBox.SelectionChangedEventArgs;

namespace PKHeX.Mobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class PKMEditor : ContentPage
    {
        public PKMEditor()
        {
            InitializeComponent();

            var ppt = new TapGestureRecognizer {NumberOfTapsRequired = 2};
            ppt.Tapped += ChangeAllPP;
            L_PP.GestureRecognizers.Add(ppt);
        }

        private PKMViewModel VM => (PKMViewModel) BindingContext;

        private async void ExportPKM(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new QRView(VM.Data)).ConfigureAwait(false);
        }

        protected override void OnDisappearing()
        {
            var pvm = (PKMViewModel)BindingContext;
            var pk = pvm.Data;
            pk.Heal();
            pk.RefreshChecksum();
            pvm.SVM?.UpdatePaneSprite();
            base.OnDisappearing();
        }

        private void HandleIV(object sender, EventArgs e)
        {
        }

        private void HandleEV(object sender, EventArgs e)
        {
        }

        private void HandleID(object sender, EventArgs e)
        {
        }

        private void HandleLevel(object sender, EventArgs e)
        {
        }

        private void HandleFriendship(object sender, EventArgs e)
        {
        }

        private void ChangeAllPP(object sender, EventArgs e)
        {
            var val = PP4.SelectedIndex == 3 ? 0 : 3;
            PP1.SelectedItem = PP2.SelectedItem = PP3.SelectedItem = PP4.SelectedItem = val;
        }

        private void CB_HeldItem_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => VM.Item = BindingUtil.GetValue(CB_HeldItem.SelectedValue);
        private void CB_Nature_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => VM.Nature = BindingUtil.GetValue(CB_Nature.SelectedValue);
        private void CB_Move1_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => VM.Move1 = BindingUtil.GetValue(CB_Move1.SelectedValue);
        private void CB_Move2_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => VM.Move2 = BindingUtil.GetValue(CB_Move2.SelectedValue);
        private void CB_Move3_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => VM.Move3 = BindingUtil.GetValue(CB_Move3.SelectedValue);
        private void CB_Move4_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => VM.Move4 = BindingUtil.GetValue(CB_Move4.SelectedValue);
        private void CB_Ball_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => VM.Ball = BindingUtil.GetValue(CB_Ball.SelectedValue);
        private void CB_EggLocation_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => VM.EggLocation = BindingUtil.GetValue(CB_Egg_Location.SelectedValue);
        private void CB_MetLocation_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => VM.MetLocation = BindingUtil.GetValue(CB_Met_Location.SelectedValue);
        private void CB_Ability_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => VM.AbilityIndex = CB_Ability.SelectedIndex;

        private void CB_Version_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.Version = BindingUtil.GetValue(CB_Version.SelectedValue);

            // binding annoyances
            var metIndex = VM.MetLocations.IndexOf(VM.MetLocations.FirstOrDefault(loc => loc.Value == VM.MetLocation));
            var eggIndex = VM.EggLocations.IndexOf(VM.EggLocations.FirstOrDefault(loc => loc.Value == VM.EggLocation));

            VM.MetLocation = VM.EggLocations[metIndex].Value;
            CB_Met_Location.SelectedItem = VM.MetLocations[metIndex];
            VM.EggLocation = VM.EggLocations[eggIndex].Value;
            CB_Egg_Location.SelectedItem = VM.EggLocations[eggIndex];
        }

        private void CB_Species_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.Species = BindingUtil.GetValue(CB_Species.SelectedValue);
            ReloadFormAbility(); // binding annoyances
        }

        private void CB_AltForm_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.Form = CB_AltForm.SelectedIndex;
            ReloadFormAbility(); // binding annoyances
        }

        private void ReloadFormAbility()
        {
            // binding annoyances
            CB_Ability.SelectedItem = VM.Abilities[VM.AbilityIndex];
            CB_AltForm.SelectedItem = VM.Forms[VM.Form];
        }
    }
}