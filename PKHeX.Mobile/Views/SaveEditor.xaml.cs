using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Acr.UserDialogs;

using PKHeX.Core;
using PKHeX.Core.AutoMod;
using PKHeX.Drawing;
using PKHeX.Mobile.Logic;
using PKHeX.ViewModels;

using SkiaSharp.Views.Forms;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace PKHeX.Mobile.Views
{
    [DesignTimeVisible(false)]
    public partial class SaveEditor : ContentPage
    {
        public SaveEditor()
        {
            InitializeComponent();
            VM.PropertyChanged += SpecialHandleChange;

            BoxSprites = new[]
            {
                PB_01, PB_02, PB_03, PB_04, PB_05, PB_06,
                PB_07, PB_08, PB_09, PB_10, PB_11, PB_12,
                PB_13, PB_14, PB_15, PB_16, PB_17, PB_18,
                PB_19, PB_20, PB_21, PB_22, PB_23, PB_24,
                PB_25, PB_26, PB_27, PB_28, PB_29, PB_30,
            };
            PartySprites = new[]
            {
                PP_01, PP_02, PP_03, PP_04, PP_05, PP_06,
            };

            var grPane = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            grPane.Tapped += (s, e) => EditPanePKM();
            PB_00.GestureRecognizers.Add(grPane);

            var grPV = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            grPV.Tapped += (s, e) => UpdateSelectedPKM(VM.Pane);
            PB_00.GestureRecognizers.Add(grPV);

            var grSB = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            grSB.Tapped += SelectBoxPKM;
            foreach (var pb in BoxSprites)
                pb.GestureRecognizers.Add(grSB);

            var grSP = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            grSP.Tapped += SelectPartyPKM;
            foreach (var pb in PartySprites)
                pb.GestureRecognizers.Add(grSP);

            var grB = new TapGestureRecognizer {NumberOfTapsRequired = 2};
            grB.Tapped += TapSpriteBox;
            foreach (var pb in BoxSprites)
                pb.GestureRecognizers.Add(grB);

            var grP = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            grP.Tapped += TapSpriteParty;
            foreach (var pb in PartySprites)
                pb.GestureRecognizers.Add(grP);

            B_Left.Clicked  += (s, e) => VM.CurrentBox = (VM.CurrentBox + VM.SAV.BoxCount - 1) % VM.SAV.BoxCount; // -1, wrap around
            B_Right.Clicked += (s, e) => VM.CurrentBox = (VM.CurrentBox                   + 1) % VM.SAV.BoxCount; // +1, wrap around
            CB_CurrentBox.SelectionChanged += (s, e) => VM.CurrentBox = CB_CurrentBox.SelectedIndex;
        }

        private readonly Image[] BoxSprites, PartySprites;

        private void SpecialHandleChange(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(SaveFileViewModel.Pane):
                    LoadPKM(VM.Pane, PB_00);
                    break;
                case nameof(SaveFileViewModel.CurrentBoxData):
                    var boxData = VM.CurrentBoxData;
                    for (int i = 0; i < boxData.Count; i++)
                        LoadPKM(boxData[i], BoxSprites[i]);
                    PB_Box.Source = (SKBitmapImageSource)VM.SAV.WallpaperSKBitmap(VM.CurrentBox);
                    break;
                case nameof(SaveFileViewModel.PartyData):
                    var partyData = VM.PartyData;
                    for (int i = 0; i < partyData.Count; i++)
                        LoadPKM(partyData[i], PartySprites[i]);
                    break;
                case nameof(SaveFileViewModel.CurrentBox):
                    CB_CurrentBox.SelectedValue = VM.BoxNames[VM.CurrentBox];
                    break;
            }
        }

        private void SelectBoxPKM(object sender, EventArgs e)
        {
            int index = Array.IndexOf(BoxSprites, (Image)sender);
            var arr = VM.CurrentBoxData;
            if ((uint) index >= arr.Count)
                return;
            UpdateSelectedPKM(arr[index]);
        }

        private void SelectPartyPKM(object sender, EventArgs e)
        {
            int index = Array.IndexOf(PartySprites, (Image)sender);
            var arr = VM.PartyData;
            if ((uint)index >= arr.Count)
                return;
            UpdateSelectedPKM(arr[index]);
        }

        private async void TapSpriteBox(object sender, EventArgs e)
        {
            var img = (Image)sender;
            int index = Array.IndexOf(BoxSprites, (Image)sender);
            var arr = VM.CurrentBoxData;
            if ((uint)index >= arr.Count)
                return;

            await TapStoredPKM(arr, index, img).ConfigureAwait(false);
        }

        private async void TapSpriteParty(object sender, EventArgs e)
        {
            var img = (Image)sender;
            int index = Array.IndexOf(PartySprites, (Image)sender);
            var arr = VM.PartyData;
            if ((uint)index >= arr.Count)
                return;

            await TapStoredPKM(arr, index, img).ConfigureAwait(false);
        }

        private void UpdateSelectedPKM(PKM pkm) => VM.Selected = pkm;

        private async Task TapStoredPKM(IList<PKM> arr, int index, Image img)
        {
            var pk = arr[index];
            var selected = await GetSimpleOption(pk, index).ConfigureAwait(false);
            if (selected < 0)
                return; // cancel

            switch (selected)
            {
                case 0: // View
                    VM.Pane = pk.Clone();
                    await UserDialogs.Instance.AlertAsync(new ShowdownSet(pk).Text).ConfigureAwait(false);
                    return;
                case 1: // Set
                    LoadPKM(arr[index] = VM.Pane.Clone(), img);
                    return;
                case 2: // Delete
                    LoadPKM(arr[index] = VM.SAV.BlankPKM, img);
                    return;
            }

            selected = await GetAdvancedOption(pk, index).ConfigureAwait(false);
            if (selected < 0)
                return;

            switch (selected)
            {
                case 0:
                    VM.Pane = pk.Clone();
                    EditPanePKM();
                    return;
                case 1:
                    await DisplayLegality(pk).ConfigureAwait(false);
                    return;
                case 2:
                    Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new QRView(pk)).ConfigureAwait(false));
                    return;
                case 3:
                    await ExportToClipboard(pk).ConfigureAwait(false);
                    return;
                default:
                    await UserDialogs.Instance.AlertAsync("Not implemented!").ConfigureAwait(false);
                    return;
            }
        }

        private void EditPanePKM()
        {
            var vm = new PKMViewModel(VM);
            var ed = new PKMEditor {BindingContext = vm};
            Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(page: ed, true).ConfigureAwait(false));
        }

        private static async Task<int> GetSimpleOption(PKM pk, int index)
        {
            const string o1 = "Pop";
            const string o2 = "Set";
            const string o3 = "Delete";
            const string o4 = "Advanced";
            var options = new[] {o1, o2, o3, o4};

            var title = index < 0 ? "Select action..." : $"{pk.Nickname} ({pk.Species}) [Slot {index + 1}]";
            var result = await UserDialogs.Instance.ActionSheetAsync(title, "Cancel", null, null, options)
                .ConfigureAwait(false);
            return Array.IndexOf(options, result);
        }

        private static async Task<int> GetAdvancedOption(PKM pk, int index)
        {
            const string o1 = "Edit";
            const string o2 = "Legality";
            const string o3 = "View QR";
            const string o4 = "Share";
            var options = new[] {o1, o2, o3, o4};

            var title = index < 0 ? "Select action..." : $"{pk.Nickname} ({pk.Species}) [Slot {index + 1}]";
            var result = await UserDialogs.Instance.ActionSheetAsync(title, "Cancel", null, null, options)
                .ConfigureAwait(false);
            return Array.IndexOf(options, result);
        }

        private static async Task DisplayLegality(PKM pk)
        {
            var la = new LegalityAnalysis(pk);
            await UserDialogs.Instance.AlertAsync($"Legality Report for {pk.Nickname}:" + Environment.NewLine + la.Report()).ConfigureAwait(false);
        }

        // ReSharper disable ImpureMethodCallOnReadonlyValueField
        private static readonly Color EmptySlot = Color.Aquamarine.MultiplyAlpha(0.3);
        private static readonly Color BadEggSlot = Color.Red.MultiplyAlpha(0.3);
        private static readonly Color OKSlot = Color.Transparent;

        private void LoadPKM(PKM pkm, Image img)
        {
            if (pkm.Species == 0)
            {
                img.BackgroundColor = EmptySlot;
                Device.BeginInvokeOnMainThread(() => img.Source = null);
                return;
            }

            if (!pkm.Valid)
            {
                img.BackgroundColor = BadEggSlot;
                Device.BeginInvokeOnMainThread(() => img.Source = null);
                return;
            }

            img.BackgroundColor = OKSlot;

            var sprite = (SKBitmapImageSource)pkm.Sprite(VM.SAV, -1, -1, true);
            Device.BeginInvokeOnMainThread(() => img.Source = sprite);
        }

        private async void ExportSAV(object sender, EventArgs e)
        {
            VM.FinalizeExport();
            var sav = VM.SAV;
            await Logic.FileUtil.ExportSAV(sav).ConfigureAwait(false);
        }

        private Image GetBoxImage(int index) => index == -1 ? PB_00 : BoxSprites[index];
        private Image GetPartyImage(int index) => index == -1 ? PB_00 : BoxSprites[index];

        // doesn't work
        protected override bool OnBackButtonPressed()
        {
            var mdl = VM;
            if (mdl.SAV.State.Edited)
            {
                bool exit = true;
                var config = new ConfirmConfig
                {
                    Title = "Unsaved changes. Exit?",
                    OkText = "Continue",
                    CancelText = "Cancel",
                    OnAction = x => exit = x
                };
                using (UserDialogs.Instance.Confirm(config))
                {
                    if (!exit)
                        return true;
                }
            }
            base.OnBackButtonPressed();
            return false;
        }

        private static async Task ExportToClipboard(PKM pk)
        {
            try
            {
                await Clipboard.SetTextAsync(Convert.ToBase64String(pk.DecryptedPartyData)).ConfigureAwait(false);
                await UserDialogs.Instance
                    .AlertAsync("Set the PKM data to the device clipboard, paste into a text box to share!")
                    .ConfigureAwait(false);
            }
            catch
            {
                await UserDialogs.Instance
                    .AlertAsync("Failed to set data to Clipboard.")
                    .ConfigureAwait(false);
            }
        }

        private async Task<PKM> ImportFromClipboard()
        {
            var txt = await DataUtil.GetClipboardText().ConfigureAwait(false);
            if (txt == null)
                return null;

            try
            {
                var data = Convert.FromBase64String(txt);
                var pkm = PKMConverter.GetPKMfromBytes(data, VM.SAV.Generation);
                if (pkm == null)
                {
                    await UserDialogs.Instance
                        .AlertAsync("Invalid data in device Clipboard!")
                        .ConfigureAwait(false);
                    return null;
                }

                var converted = PKMConverter.ConvertToType(pkm, VM.SAV.PKMType, out var c);
                if (converted == null)
                {
                    await UserDialogs.Instance
                        .AlertAsync(c)
                        .ConfigureAwait(false);
                    return null;
                }

                return converted;
            }
            catch
            {
                await UserDialogs.Instance
                    .AlertAsync("Invalid data in device Clipboard!")
                    .ConfigureAwait(false);
                return null;
            }
        }

        private async Task<PKM> ShowdownImportFromClipboard(PKM template)
        {
            var txt = await DataUtil.GetClipboardText().ConfigureAwait(false);
            if (txt == null)
                return null;

            var set = new ShowdownSet(txt);

            if (set.Species < 0)
            {
                await UserDialogs.Instance
                    .AlertAsync("No valid Showdown Set data was found. Please try again.")
                    .ConfigureAwait(false);
                return null;
            }

            var pkm = template.Clone();
            if (set.Nickname?.Length > pkm.NickLength)
                set.Nickname = set.Nickname.Substring(0, pkm.NickLength);

            pkm.ApplySetDetails(set);
            return pkm;
        }

        private void B_Money_Max_Click(object sender, EventArgs e)
        {
            var sav = VM.SAV;

            int max = sav.MaxMoney;
            if (max < 0)
                return;
            var current = sav.Money;
            sav.Money = (uint)max;

            UserDialogs.Instance.Alert($"Old Money: {current:C0}{Environment.NewLine}New Money: {max:C0}", "Updated!");
        }

        private void B_Inventory_All_Click(object sender, EventArgs e)
        {
            var sav = VM.SAV;

            var items = sav.Inventory;
            if (items.Count == 0)
            {
                UserDialogs.Instance.Alert("Save file does not have an editable Inventory!");
                return;
            }

            var types = new[]
            {
                InventoryType.Items,
                InventoryType.Balls,
                InventoryType.BattleItems,

                InventoryType.Berries,
                InventoryType.Candy,
                InventoryType.MailItems,
                //InventoryType.TMHMs,
            };
            foreach (var p in items.Where(z => types.Contains(z.Type)))
                p.GiveAllItems(sav);

            sav.Inventory = items;
            UserDialogs.Instance.Alert("All items have been given (where appropriate)!", "Updated!");
        }

        private async void B_ScanPKM_Click(object sender, EventArgs e)
        {
            var pkm = await QRPageUtil.ScanQRPKM(VM.SAV, Navigation).ConfigureAwait(false);
            if (pkm == null)
                return;

            VM.Pane = pkm;
            VM.UpdatePaneSprite();
        }

        private async void B_ImportPKM_Click(object sender, EventArgs e)
        {
            var pkm = await ImportFromClipboard().ConfigureAwait(false);
            if (pkm == null)
                return;
            VM.Pane = pkm;
            VM.UpdatePaneSprite();
        }

        private async void B_ExportPKM_Click(object sender, EventArgs e) => await ExportToClipboard(VM.Pane).ConfigureAwait(false);

        private void B_EditPanePKM_Click(object sender, EventArgs e) => EditPanePKM();

        private async void B_Legalize_Click(object sender, EventArgs e)
        {
            var pkm = Legalize(VM.Pane.Clone());
            if (pkm == null)
            {
                await UserDialogs.Instance.AlertAsync("Unable to legalize.").ConfigureAwait(false);
                return;
            }
            VM.Pane = pkm;
            VM.UpdatePaneSprite();
        }

        private async void B_ExportQR_Click(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new QRView(VM.Pane)).ConfigureAwait(false);
        }

        private async void B_ImportShowdownPKM_Click(object sender, EventArgs e)
        {
            var pkm = await ShowdownImportFromClipboard(VM.Pane).ConfigureAwait(false);
            if (pkm == null)
                return;
            VM.Pane = pkm;
            VM.UpdatePaneSprite();
        }

        private async void B_ExportShowdownPKM_Click(object sender, EventArgs e)
        {
            try
            {
                await Clipboard.SetTextAsync(ShowdownParsing.GetShowdownText(VM.Pane)).ConfigureAwait(false);
                await UserDialogs.Instance
                    .AlertAsync("Set the exported Showdown Set to the device clipboard!")
                    .ConfigureAwait(false);
            }
            catch
            {
                await UserDialogs.Instance
                    .AlertAsync("Failed to set Showdown Set to Clipboard.")
                    .ConfigureAwait(false);
            }
        }

        private static PKM Legalize(PKM pk)
        {
            try
            {
                var current = new LegalityAnalysis(pk);
                if (current.Valid)
                    return null;

                var legal = pk.Legalize();
                legal.RefreshChecksum();
                var la = new LegalityAnalysis(legal);
                return la.Valid ? legal : null;
            }
            catch
            {
                return null;
            }
        }
    }
}