using System;
using System.Collections.ObjectModel;
using PKHeX.Core;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace PKHeX.ViewModels
{
    public class PKMViewModel : BaseViewModel
    {
        public PKMViewModel()
        {
            Title = "PKM Editor [Debug]";
            Source = new ComboSource();
            FeatureFlags = new FeatureFlags(new PK7());
        }

        public PKMViewModel(SaveFileViewModel svm)
        {
            Title = "PKM Editor";
            SVM = svm;
            LoadData(svm.Pane, svm.SAV);
        }

        private void LoadData(PKM pkm, SaveFile sav)
        {
            SAV = sav;
            Data = pkm;
            GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources);
            Source = new ComboSource();

            FeatureFlags = new FeatureFlags(Data);
            LoadPKMFields();
        }

        private void LoadPKMFields()
        {
            species = Data.Species;
            form = Data.Form;
            abilIndex = Data.AbilityNumber >> 1;
            level = Data.CurrentLevel;
            item = Data.HeldItem;
            nature = Data.Nature;
            move1 = Data.Move1;
            move2 = Data.Move2;
            move3 = Data.Move3;
            move4 = Data.Move4;
            version = Data.Version;
            ball = Data.Ball;
            metloc = Data.Met_Location;
            eggloc = Data.Egg_Location;
            Personal = Data.PersonalInfo;
            gameGroup = GameUtil.GetMetLocationVersionGroup((GameVersion)version);
            RefreshMetLocations();
            RefreshAbilities();
            RefreshForms();
        }

        private void RefreshForms()
        {
            var list = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolASCII, Data.Format);
            Forms = new ObservableCollection<string>(list);
        }

        private void RefreshAbilities()
        {
            var list = GameInfo.FilteredSources.GetAbilityList(Data);
            Abilities = new ObservableCollection<ComboItem>(list);
        }

        public SaveFileViewModel SVM { get; }

        public SaveFile SAV { get; private set; }
        public PKM Data { get; private set; }
        public ComboSource Source { get; private set; }
        public FeatureFlags FeatureFlags { get; private set; }
        public PersonalInfo Personal { get; private set; }

        public ObservableCollection<ComboItem> MetLocations { get; set; }
        public ObservableCollection<ComboItem> EggLocations { get; set; }

        public ObservableCollection<ComboItem> Abilities { get; set; }
        public ObservableCollection<string> Forms { get; set; }

        public string PKMTitle => GetProgramTitle(Data);
        public bool Legal => RefreshLegalStatus();

        public string OT_Name
        {
            get => Data.OT_Name;
            set
            {
                if (Data.OT_Name == value)
                    return;
                Data.OT_Name = value;
            }
        }

        public string HT_Name
        {
            get => Data.HT_Name;
            set
            {
                if (Data.HT_Name == value)
                    return;
                Data.HT_Name = value;
            }
        }

        public string Nickname
        {
            get => Data.Nickname;
            set
            {
                if (Data.Nickname == value)
                    return;
                Data.SetNickname(value);
                OnPropertyChanged(nameof(IsNicknamed));
                OnPropertyChanged(nameof(PKMTitle));
            }
        }

        public bool IsNicknamed
        {
            get => Data.IsNicknamed;
            set
            {
                if (Data.IsNicknamed == value)
                    return;
                if (!value)
                    ResetNickname();
            }
        }

        private void ResetNickname()
        {
            Data.SetNickname(string.Empty);
            OnPropertyChanged(nameof(Nickname));
        }

        private LegalityAnalysis Analysis;

        private bool RefreshLegalStatus()
        {
            Analysis = new LegalityAnalysis(Data);

            Move1Legal = Analysis.Info.Moves[0].Valid;
            Move2Legal = Analysis.Info.Moves[1].Valid;
            Move3Legal = Analysis.Info.Moves[2].Valid;
            Move4Legal = Analysis.Info.Moves[3].Valid;

            RelearnMove1Legal = Analysis.Info.Relearn[0].Valid;
            RelearnMove2Legal = Analysis.Info.Relearn[1].Valid;
            RelearnMove3Legal = Analysis.Info.Relearn[2].Valid;
            RelearnMove4Legal = Analysis.Info.Relearn[3].Valid;

            return Analysis.Valid;
        }

        public bool Move1Legal { get; private set; }
        public bool Move2Legal { get; private set; }
        public bool Move3Legal { get; private set; }
        public bool Move4Legal { get; private set; }

        public bool RelearnMove1Legal { get; private set; }
        public bool RelearnMove2Legal { get; private set; }
        public bool RelearnMove3Legal { get; private set; }
        public bool RelearnMove4Legal { get; private set; }

        private int species;
        private int form;
        private int abilIndex;
        private int nature;
        private int move1;
        private int move2;
        private int move3;
        private int move4;
        private int version;
        private int ball;
        private int level;

        private GameVersion gameGroup;
        private int metloc;
        private int eggloc;

        public int Species
        {
            get => species;
            set
            {
                if (!SetProperty(ref species, value))
                    return;
                Data.Species = value;
                if (!IsNicknamed)
                    ResetNickname();
                ChangeSpecies();
            }
        }

        public int Form
        {
            get => form;
            set
            {
                if (!SetProperty(ref form, value))
                    return;
                Data.Form = value;
                ChangeForm();
            }
        }

        public int AbilityIndex
        {
            get => abilIndex;
            set
            {
                if (!SetProperty(ref abilIndex, value))
                    return;
                Data.RefreshAbility(value);
            }
        }

        public int CurrentLevel
        {
            get => level;
            set
            {
                if (!SetProperty(ref level, value))
                    return;
                Data.CurrentLevel = value;
            }
        }

        private int item;

        public int Item
        {
            get => item;
            set
            {
                if (!SetProperty(ref item, value))
                    return;
                Data.HeldItem = value;
            }
        }

        public int Nature
        {
            get => nature;
            set
            {
                if (!SetProperty(ref nature, value))
                    return;
                Data.Nature = value;
            }
        }

        public int Move1
        {
            get => move1;
            set
            {
                if (!SetProperty(ref move1, value))
                    return;
                Data.Move1 = value;
            }
        }

        public int Move2
        {
            get => move2;
            set
            {
                if (!SetProperty(ref move2, value))
                    return;
                Data.Move2 = value;
            }
        }

        public int Move3
        {
            get => move3;
            set
            {
                if (!SetProperty(ref move3, value))
                    return;
                Data.Move3 = value;
            }
        }

        public int Move4
        {
            get => move4;
            set
            {
                if (!SetProperty(ref move4, value))
                    return;
                Data.Move4 = value;
            }
        }

        public int Version
        {
            get => version;
            set
            {
                if (!SetProperty(ref version, value))
                    return;
                Data.Version = value;
                GameGroup = GameUtil.GetMetLocationVersionGroup((GameVersion)value);
            }
        }

        public GameVersion GameGroup
        {
            get => gameGroup;
            set
            {
                if (!SetProperty(ref gameGroup, value))
                    return;

                RefreshMetLocations();
                OnPropertyChanged(nameof(MetLocations));
                OnPropertyChanged(nameof(EggLocations));
            }
        }

        public int Ball
        {
            get => ball;
            set
            {
                if (!SetProperty(ref ball, value))
                    return;
                Data.Ball = value;
                OnPropertyChanged(nameof(BallSprite));
            }
        }

        public int MetLocation
        {
            get => metloc;
            set
            {
                if (!SetProperty(ref metloc, value))
                    return;
                Data.Met_Location = value;
            }
        }

        public int EggLocation
        {
            get => eggloc;
            set
            {
                if (!SetProperty(ref eggloc, value))
                    return;
                Data.Egg_Location = value;
            }
        }

        public DateTime MetDate
        {
            get => Data.MetDate ?? new DateTime(2000, 1, 1);
            set
            {
                if (value == Data.MetDate)
                    return;
                Data.MetDate = value;
                OnPropertyChanged(nameof(MetDate));
            }
        }

        public DateTime EggMetDate
        {
            get => Data.EggMetDate ?? new DateTime(2000, 1, 1);
            set
            {
                if (value == Data.EggMetDate)
                    return;
                Data.EggMetDate = value;
                OnPropertyChanged(nameof(EggMetDate));
            }
        }

        public ImageSource BallSprite => (SKBitmapImageSource)Drawing.SpriteUtil.GetBallSprite(ball);

        private void ChangeSpecies()
        {
            Personal = Data.PersonalInfo;
            SetAbilityList();
            SetForms();
            Data.CurrentLevel = level;
        }

        private void ChangeForm()
        {
            Personal = Data.PersonalInfo;
            SetAbilityList();
            Data.CurrentLevel = level;
        }

        private void SetForms()
        {
            RefreshForms();
            OnPropertyChanged(nameof(Forms));
            if (Form > 0 && Forms.Count == 0)
                Form = 0;
        }

        private void SetAbilityList()
        {
            RefreshAbilities();
            OnPropertyChanged(nameof(Abilities));
        }

        private void RefreshMetLocations()
        {
            MetLocations = new ObservableCollection<ComboItem>(GameInfo.GetLocationList((GameVersion)version, Data.Format));
            EggLocations = new ObservableCollection<ComboItem>(GameInfo.GetLocationList((GameVersion)version, Data.Format, true));
        }

        private static string GetProgramTitle(PKM pkm)
        {
            var str = pkm.Nickname;
            if (!string.IsNullOrWhiteSpace(str))
                return str;
            int lang = GameLanguage.GetLanguageIndex(GameInfo.CurrentLanguage);
            if (lang <= 0)
                lang = (int)LanguageID.English;
            return SpeciesName.GetSpeciesName(pkm.Species, lang);
        }
    }
}