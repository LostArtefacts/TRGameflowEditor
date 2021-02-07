using System.ComponentModel;
using System.Runtime.CompilerServices;
using TRGE.Core;

namespace TRGE.View.Model
{
    public class EditorOptions : INotifyPropertyChanged
    {
        #region Properties
        private bool _titleEnabled, _levelSelectEnabled, _saveLoadEnabled, _optionRingEnabled;
        private bool _fmvsEnabled, _cutscenesEnabled, _startAnimationsEnabled, _cheatsEnabled, _dozyViable, _dozyEnabled;
        private bool _demosEnabled, _trainingEnabled;
        private int _demoDelay;
        private bool _useDefaultLevelSequence, _useManualLevelSequence;
        private bool _useDefaultUnarmedLevels, _useManualUnarmedLevels;
        private bool _useDefaultAmmolessLevels, _useManualAmmolessLevels;

        private bool _secretRewardsViable;
        private bool _useDefaultSecretBonuses, _useManualSecretBonuses;

        private bool _sunsetsViable;
        private bool _useDefaultSunsets, _useManualSunsets;

        private bool _useDefaultAudio, _useManualAudio;

        public bool TitleEnabled
        {
            get => _titleEnabled;
            set
            {
                _titleEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool LevelSelectEnabled
        {
            get => _levelSelectEnabled;
            set
            {
                _levelSelectEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool SaveLoadEnabled
        {
            get => _saveLoadEnabled;
            set
            {
                _saveLoadEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool OptionRingEnabled
        {
            get => _optionRingEnabled;
            set
            {
                _optionRingEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool FMVsEnabled
        {
            get => _fmvsEnabled;
            set
            {
                _fmvsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool CutscenesEnabled
        {
            get => _cutscenesEnabled;
            set
            {
                _cutscenesEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool StartAnimationsEnabled
        {
            get => _startAnimationsEnabled;
            set
            {
                _startAnimationsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool CheatsEnabled
        {
            get => _cheatsEnabled;
            set
            {
                _cheatsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool DozyViable
        {
            get => _dozyViable;
            private set
            {
                _dozyViable = value;
                OnPropertyChanged();
            }
        }

        public bool DozyEnabled
        {
            get => _dozyEnabled;
            set
            {
                _dozyEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool DemosEnabled
        {
            get => _demosEnabled;
            set
            {
                _demosEnabled = value;
                OnPropertyChanged();
            }
        }

        public int DemoDelay
        {
            get => _demoDelay;
            set
            {
                _demoDelay = value;
                OnPropertyChanged();
            }
        }

        public bool TrainingEnabled
        {
            get => _trainingEnabled;
            set
            {
                _trainingEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool UseDefaultLevelSequence
        {
            get => _useDefaultLevelSequence;
            set
            {
                _useDefaultLevelSequence = value;
                OnPropertyChanged();
            }
        }

        public bool UseManualLevelSequence
        {
            get => _useManualLevelSequence;
            set
            {
                _useManualLevelSequence = value;
                OnPropertyChanged();
            }
        }

        public bool UseDefaultUnarmed
        {
            get => _useDefaultUnarmedLevels;
            set
            {
                _useDefaultUnarmedLevels = value;
                OnPropertyChanged();
            }
        }

        public bool UseManualUnarmed
        {
            get => _useManualUnarmedLevels;
            set
            {
                _useManualUnarmedLevels = value;
                OnPropertyChanged();
            }
        }

        public bool UseDefaultAmmoless
        {
            get => _useDefaultAmmolessLevels;
            set
            {
                _useDefaultAmmolessLevels = value;
                OnPropertyChanged();
            }
        }

        public bool UseManualAmmoless
        {
            get => _useManualAmmolessLevels;
            set
            {
                _useManualAmmolessLevels = value;
                OnPropertyChanged();
            }
        }

        public bool SecretRewardsViable
        {
            get => _secretRewardsViable;
            private set
            {
                _secretRewardsViable = value;
                OnPropertyChanged();
            }
        }

        public bool UseDefaultBonuses
        {
            get => _useDefaultSecretBonuses;
            set
            {
                _useDefaultSecretBonuses = value;
                OnPropertyChanged();
            }
        }

        public bool UseManualBonuses
        {
            get => _useManualSecretBonuses;
            set
            {
                _useManualSecretBonuses = value;
                OnPropertyChanged();
            }
        }

        public bool SunsetsViable
        {
            get => _sunsetsViable;
            private set
            {
                _sunsetsViable = value;
                OnPropertyChanged();
            }
        }

        public bool UseDefaultSunsets
        {
            get => _useDefaultSunsets;
            set
            {
                _useDefaultSunsets = value;
                OnPropertyChanged();
            }
        }

        public bool UseManualSunsets
        {
            get => _useManualSunsets;
            set
            {
                _useManualSunsets = value;
                OnPropertyChanged();
            }
        }

        public bool UseDefaultAudio
        {
            get => _useDefaultAudio;
            set
            {
                _useDefaultAudio = value;
                OnPropertyChanged();
            }
        }

        public bool UseManualAudio
        {
            get => _useManualAudio;
            set
            {
                _useManualAudio = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public void Load(TR23ScriptEditor editor)
        {
            TitleEnabled = editor.TitleScreenEnabled;
            LevelSelectEnabled = editor.LevelSelectEnabled;
            SaveLoadEnabled = editor.SaveLoadEnabled;
            OptionRingEnabled = editor.OptionRingEnabled;

            FMVsEnabled = editor.LevelsHaveFMV;
            CutscenesEnabled = editor.LevelsHaveCutScenes;
            StartAnimationsEnabled = editor.LevelsHaveStartAnimation;
            CheatsEnabled = editor.CheatsEnabled;
            DozyViable = editor.DozySupported;
            DozyEnabled = editor.DozyEnabled;

            DemosEnabled = editor.DemosEnabled;
            DemoDelay = (int)editor.DemoTime;
            TrainingEnabled = editor.GymEnabled;

            UseManualLevelSequence = editor.LevelSequencingOrganisation == Organisation.Manual;
            UseDefaultLevelSequence = !UseManualLevelSequence;

            UseManualUnarmed = editor.UnarmedLevelOrganisation == Organisation.Manual;
            UseDefaultUnarmed = !UseManualUnarmed;

            UseManualAmmoless = editor.AmmolessLevelOrganisation == Organisation.Manual;
            UseDefaultAmmoless = !UseManualAmmoless;

            SecretRewardsViable = editor.CanOrganiseBonuses;
            UseManualBonuses = editor.SecretBonusOrganisation == Organisation.Manual;
            UseDefaultBonuses = !UseManualBonuses;

            SunsetsViable = editor.CanSetSunsets;
            UseManualSunsets = editor.LevelSunsetOrganisation == Organisation.Manual;
            UseDefaultSunsets = !UseManualSunsets;

            UseManualAudio = editor.GameTrackOrganisation == Organisation.Manual;
            UseDefaultAudio = !UseManualAudio;
        }

        public void Save(TR23ScriptEditor editor)
        {
            editor.TitleScreenEnabled = TitleEnabled;
            editor.LevelSelectEnabled = LevelSelectEnabled;
            editor.SaveLoadEnabled = SaveLoadEnabled;
            editor.OptionRingEnabled = OptionRingEnabled;

            editor.LevelsHaveFMV = editor.FrontEndHasFMV = FMVsEnabled;
            editor.LevelsHaveCutScenes = CutscenesEnabled;
            editor.LevelsHaveStartAnimation = StartAnimationsEnabled;
            editor.CheatsEnabled = CheatsEnabled;
            editor.DozyEnabled = DozyEnabled;

            editor.DemosEnabled = DemosEnabled;
            editor.DemoTime = (uint)DemoDelay;
            editor.GymEnabled = TrainingEnabled;

            editor.LevelSequencingOrganisation = UseManualLevelSequence ? Organisation.Manual : Organisation.Default;

            editor.UnarmedLevelOrganisation = UseManualUnarmed ? Organisation.Manual : Organisation.Default;

            editor.AmmolessLevelOrganisation = UseManualAmmoless ? Organisation.Manual : Organisation.Default;

            if (SecretRewardsViable)
            {
                editor.SecretBonusOrganisation = UseManualBonuses ? Organisation.Manual : Organisation.Default;
            }

            if (SunsetsViable)
            {
                editor.LevelSunsetOrganisation = UseManualSunsets ? Organisation.Manual : Organisation.Default;
            }

            editor.GameTrackOrganisation = UseManualAudio ? Organisation.Manual : Organisation.Default;
        }
    }
}