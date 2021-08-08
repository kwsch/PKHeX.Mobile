using System.Collections.Generic;
using System.Linq;
using MvvmHelpers;
using PKHeX.Core;
using PKHeX.Mobile.Logic;

namespace PKHeX.ViewModels
{
    public class SaveFileViewModel : BaseViewModel
    {
        public void Initialize(SaveFile s)
        {
            SAV = s;
            if (s.HasBox)
            {
                BoxNames.AddRange(Enumerable.Range(0, s.BoxCount).Select(s.GetBoxName).ToArray());
                int box = s.CurrentBox;
                CurrentBox = box <= 0 ? 0 : box % s.BoxCount;
            }

            var pk = s.BlankPKM;
            {
                pk.Species = 25; pk.Nickname = "Test"; pk.Language = 2; pk.IsNicknamed = true; pk.HeldItem = 1;
                pk.Move1 = 1; pk.Move2 = 4; pk.Move3 = 8; pk.Move4 = 17;
                pk.PID = Util.Rand32();
            }
            pk.Heal();
            pk.RefreshChecksum();

            Pane = pk;

            IEnumerable<PKM> GetFullParty(IEnumerable<PKM> init, int count)
            {
                int ctr = 0;
                foreach (var p in init)
                {
                    ctr++;
                    yield return p;
                }
                for (int i = ctr; i < count; i++)
                    yield return s.BlankPKM;
            }
            PartyData = new ObservableRangeCollection<PKM>(GetFullParty(SAV.PartyData, 6));
        }

        private SaveFile sav;
        private PKM pane;
        private PKM selected;
        private int c_box = -1;
        private int p_box = -1;

        public SaveFile SAV
        {
            get => sav;
            set
            {
                if (sav == value)
                    return;
                sav = value;
                Title = InfoUtil.GetProgramTitle(sav);
            }
        }

        public PKM Pane
        {
            get => pane;
            set
            {
                if (pane == value)
                    return;
                pane = value;
                UpdatePaneSprite();
            }
        }

        public PKM Selected
        {
            get => selected;
            set
            {
                if (ReferenceEquals(selected, value))
                    return;
                selected = value;
            }
        }

        public void UpdatePaneSprite() => OnPropertyChanged(nameof(Pane));

        public string ShowdownText {
            get
            {
                if (selected == null) return ShowdownParsing.GetShowdownText(pane);
                return ShowdownParsing.GetShowdownText(selected);
            }
        }

        public ObservableRangeCollection<string> BoxNames { get; } = new ObservableRangeCollection<string>();

        public int CurrentBox
        {
            get => c_box;
            set
            {
                if (!SetProperty(ref c_box, value))
                    return;
                ChangeBox(value);
            }
        }

        private void ChangeBox(int index)
        {
            if (p_box >= 0)
                SAV.SetBoxData(CurrentBoxData, p_box);
            SAV.CurrentBox = p_box = index;
            CurrentBoxData = new ObservableRangeCollection<PKM>(SAV.GetBoxData(index));
        }

        public void FinalizeExport()
        {
            if (p_box >= 0)
                SAV.SetBoxData(CurrentBoxData, p_box);
            SAV.PartyData = PartyData;
        }

        public ObservableRangeCollection<PKM> CurrentBoxData { get; set; }
        public ObservableRangeCollection<PKM> PartyData { get; set; }
    }
}