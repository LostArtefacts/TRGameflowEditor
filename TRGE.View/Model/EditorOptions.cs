using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TRGE.Core;
using TRGE.View.Model.Audio;
using TRGE.View.Model.Data;

namespace TRGE.View.Model
{
    public class EditorOptions : INotifyPropertyChanged, IAudioDataProvider
    {
        #region Properties
        private TR23ScriptEditor _editor;

        private bool _titleEnabled, _levelSelectEnabled, _saveLoadEnabled, _optionRingEnabled;
        private bool _fmvsEnabled, _cutscenesEnabled, _startAnimationsEnabled, _cheatsEnabled, _dozyViable, _dozyEnabled;
        private bool _demosViable, _demosEnabled, _trainingViable, _trainingEnabled, _trainingWeaponsEnabled, _trainingSkidooViable, _trainingSkidooEnabled;
        private int _demoDelay;

        private bool _levelSequencingViable;
        private bool _useDefaultLevelSequence, _useManualLevelSequence;
        private List<Tuple<string, string>> _levelSequencing;
        private List<MutableTuple<string, string, bool>> _enabledLevelStatus;

        private bool _unarmedLevelsViable;
        private bool _useDefaultUnarmedLevels, _useManualUnarmedLevels;
        private List<MutableTuple<string, string, bool>> _unarmedLevelData;

        private bool _ammolessLevelsViable;
        private bool _useDefaultAmmolessLevels, _useManualAmmolessLevels;
        private List<MutableTuple<string, string, bool>> _ammolessLevelData;

        private bool _secretRewardsSupported, _secretRewardsViable;
        private bool _useDefaultSecretBonuses, _useManualSecretBonuses;
        private List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> _secretBonusData;

        private bool _sunsetsSupported, _sunsetsViable;
        private bool _useDefaultSunsets, _useManualSunsets;
        private List<MutableTuple<string, string, bool>> _sunsetLevelData;

        private bool _audioViable;
        private bool _useDefaultAudio, _useManualAudio;
        private List<MutableTuple<string, string, ushort>> _audioData;
        private IReadOnlyList<Tuple<ushort, string>> _allAudioTracks;

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

        public bool DemosViable
        {
            get => _demosViable;
            private set
            {
                _demosViable = value;
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

        public bool TrainingLevelViable
        {
            get => _trainingViable;
            set
            {
                _trainingViable = value;
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

        public bool TrainingWeaponsEnabled
        {
            get => _trainingWeaponsEnabled;
            set
            {
                _trainingWeaponsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool TrainingSkidooViable
        {
            get => _trainingSkidooViable;
            set
            {
                _trainingSkidooViable = value;
                OnPropertyChanged();
            }
        }

        public bool TrainingSkidooEnabled
        {
            get => _trainingSkidooEnabled;
            set
            {
                _trainingSkidooEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool LevelSequencingViable
        {
            get => _levelSequencingViable;
            private set
            {
                _levelSequencingViable = value;
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

        public LevelSequencingData LevelSequencing
        {
            get => new LevelSequencingData(_levelSequencing, _enabledLevelStatus);
            set
            {
                _levelSequencing = value.ToSequenceTupleList();
                _enabledLevelStatus = value.ToEnabledTupleList();
                OnPropertyChanged();
            }
        }

        public bool UnarmedLevelsViable
        {
            get => _unarmedLevelsViable;
            private set
            {
                _unarmedLevelsViable = value;
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

        public FlaggedLevelData UnarmedLevelData
        {
            get => new FlaggedLevelData(_unarmedLevelData);
            set
            {
                _unarmedLevelData = value.ToTupleList();
                OnPropertyChanged();
            }
        }

        public bool AmmolessLevelsViable
        {
            get => _ammolessLevelsViable;
            private set
            {
                _ammolessLevelsViable = value;
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

        public FlaggedLevelData AmmolessLevelData
        {
            get => new FlaggedLevelData(_ammolessLevelData);
            set
            {
                _ammolessLevelData = value.ToTupleList();
                OnPropertyChanged();
            }
        }

        public bool SecretRewardsSupported
        {
            get => _secretRewardsSupported;
            private set
            {
                _secretRewardsSupported = value;
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

        public GlobalSecretBonusData SecretBonusData
        {
            get => new GlobalSecretBonusData(_secretBonusData);
            set
            {
                _secretBonusData = value.ToTupleList();
                OnPropertyChanged();
            }
        }

        public bool SunsetsSupported
        {
            get => _sunsetsSupported;
            private set
            {
                _sunsetsSupported = value;
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

        public FlaggedLevelData SunsetLevelData
        {
            get => new FlaggedLevelData(_sunsetLevelData);
            set
            {
                _sunsetLevelData = value.ToTupleList();
                OnPropertyChanged();
            }
        }

        public bool AudioViable
        {
            get => _audioViable;
            private set
            {
                _audioViable = value;
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

        public AudioData GlobalAudioData
        {
            get => new AudioData(_audioData, _allAudioTracks);
            set
            {
                _audioData = value.ToTupleList();
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Viability
        public int GetUnviableCount()
        {
            int i = 0;
            if (!LevelSequencingViable) i++;
            if (!UnarmedLevelsViable) i++;
            if (!AmmolessLevelsViable) i++;
            if (SecretRewardsSupported && !SecretRewardsViable) i++;
            if (SunsetsSupported && !SunsetsViable) i++;
            if (!AudioViable) i++;
            return i;
        }

        public void SetLevelSequencingViable()
        {
            LevelSequencingViable = true;
            UseDefaultLevelSequence = true;
            UseManualLevelSequence = false;
        }

        public void SetUnarmedLevelsViable()
        {
            UnarmedLevelsViable = true;
            UseDefaultUnarmed = true;
            UseManualUnarmed = false;
        }

        public void SetAmmolessLevelsViable()
        {
            AmmolessLevelsViable = true;
            UseDefaultAmmoless = true;
            UseManualAmmoless = false;
        }

        public void SetSecretRewardsViable()
        {
            if (SecretRewardsSupported)
            {
                SecretRewardsViable = true;
                UseDefaultBonuses = true;
                UseManualBonuses = false;
            }
        }

        public void SetSunsetsViable()
        {
            if (SunsetsSupported)
            {
                SunsetsViable = true;
                UseDefaultSunsets = true;
                UseManualSunsets = false;
            }
        }

        public void SetAudioViable()
        {
            AudioViable = true;
            UseDefaultAudio = true;
            UseManualAudio = false;
        }
        #endregion

        #region IO
        public void Load(TR23ScriptEditor editor)
        {
            _editor = editor;
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

            DemosViable = editor.DemosSupported;
            DemosEnabled = editor.DemosEnabled;
            DemoDelay = (int)editor.DemoTime;
            TrainingEnabled = editor.GymEnabled;
            TrainingLevelViable = editor.GymAvailable;
            TrainingWeaponsEnabled = editor.AddGymWeapons;
            TrainingSkidooViable = editor.SkidooAvailable;
            TrainingSkidooEnabled = editor.AddGymSkidoo;

            UseManualLevelSequence = editor.LevelSequencingOrganisation == Organisation.Manual;
            UseDefaultLevelSequence = editor.LevelSequencingOrganisation == Organisation.Default;
            LevelSequencingViable = UseManualLevelSequence || UseDefaultLevelSequence; //if rando, not supported in this UI
            _levelSequencing = editor.LevelSequencing;
            _enabledLevelStatus = editor.EnabledLevelStatus;

            UseManualUnarmed = editor.UnarmedLevelOrganisation == Organisation.Manual;
            UseDefaultUnarmed = editor.UnarmedLevelOrganisation == Organisation.Default;
            UnarmedLevelsViable = UseManualUnarmed || UseDefaultUnarmed; //if rando, not supported in this UI
            _unarmedLevelData = editor.UnarmedLevelData;

            UseManualAmmoless = editor.AmmolessLevelOrganisation == Organisation.Manual;
            UseDefaultAmmoless = editor.AmmolessLevelOrganisation == Organisation.Default;
            AmmolessLevelsViable = UseManualAmmoless || UseDefaultAmmoless; //if rando, not supported in this UI
            _ammolessLevelData = editor.AmmolessLevelData;

            SecretRewardsSupported = editor.CanOrganiseBonuses;
            UseManualBonuses = editor.SecretBonusOrganisation == Organisation.Manual;
            UseDefaultBonuses = editor.SecretBonusOrganisation == Organisation.Default;
            SecretRewardsViable = UseManualBonuses || UseDefaultBonuses; //if rando, not supported in this UI
            _secretBonusData = editor.LevelSecretBonusData;

            SunsetsSupported = editor.CanSetSunsets;
            UseManualSunsets = editor.LevelSunsetOrganisation == Organisation.Manual;
            UseDefaultSunsets = editor.LevelSunsetOrganisation == Organisation.Default;
            SunsetsViable = UseManualSunsets || UseDefaultSunsets; //if rando, not supported in this UI
            _sunsetLevelData = editor.LevelSunsetData;

            UseManualAudio = editor.GameTrackOrganisation == Organisation.Manual;
            UseDefaultAudio = editor.GameTrackOrganisation == Organisation.Default;
            AudioViable = UseManualAudio || UseDefaultAudio; //if rando, not supported in this UI
            _audioData = editor.GameTrackData;

            _allAudioTracks = editor.AllGameTracks;
        }

        public void Unload()
        {
            _editor = null;
        }

        public void Save()
        {
            _editor.TitleScreenEnabled = TitleEnabled;
            _editor.LevelSelectEnabled = LevelSelectEnabled;
            _editor.SaveLoadEnabled = SaveLoadEnabled;
            _editor.OptionRingEnabled = OptionRingEnabled;

            _editor.LevelsHaveFMV = _editor.FrontEndHasFMV = FMVsEnabled;
            _editor.LevelsHaveCutScenes = CutscenesEnabled;
            _editor.LevelsHaveStartAnimation = StartAnimationsEnabled;
            _editor.CheatsEnabled = CheatsEnabled;
            _editor.DozyEnabled = DozyEnabled;

            _editor.DemosEnabled = DemosEnabled;
            _editor.DemoTime = (uint)DemoDelay;
            _editor.GymEnabled = TrainingEnabled;
            _editor.AddGymWeapons = TrainingWeaponsEnabled;
            _editor.AddGymSkidoo = TrainingSkidooEnabled;

            if (LevelSequencingViable)
            {
                _editor.LevelSequencingOrganisation = _editor.EnabledLevelOrganisation = UseManualLevelSequence ? Organisation.Manual : Organisation.Default;
                if (UseManualLevelSequence)
                {
                    _editor.LevelSequencing = _levelSequencing;
                    _editor.EnabledLevelStatus = _enabledLevelStatus;
                }
            }

            if (UnarmedLevelsViable)
            {
                _editor.UnarmedLevelOrganisation = UseManualUnarmed ? Organisation.Manual : Organisation.Default;
                if (UseManualUnarmed)
                {
                    _editor.UnarmedLevelData = _unarmedLevelData;
                }
            }

            if (AmmolessLevelsViable)
            {
                _editor.AmmolessLevelOrganisation = UseManualAmmoless ? Organisation.Manual : Organisation.Default;
                if (UseManualAmmoless)
                {
                    _editor.AmmolessLevelData = _ammolessLevelData;
                }
            }

            if (SecretRewardsSupported && SecretRewardsViable)
            {
                _editor.SecretBonusOrganisation = UseManualBonuses ? Organisation.Manual : Organisation.Default;
                if (UseManualBonuses)
                {
                    _editor.LevelSecretBonusData = _secretBonusData;
                }
            }

            if (SunsetsSupported && SunsetsViable)
            {
                _editor.LevelSunsetOrganisation = UseManualSunsets ? Organisation.Manual : Organisation.Default;
                if (UseManualSunsets)
                {
                    _editor.LevelSunsetData = _sunsetLevelData;
                }
            }

            if (AudioViable)
            {
                _editor.GameTrackOrganisation = UseManualAudio ? Organisation.Manual : Organisation.Default;
                if (UseManualAudio)
                {
                    _editor.GameTrackData = _audioData;
                }
            }
        }
        #endregion

        #region IAudioDataProvider
        public byte[] GetAudioTrackData(AudioTrack track)
        {
            return _editor.GetTrackData(track.ID);
        }

        public AudioData GetAudioData()
        {
            return GlobalAudioData;
        }
        #endregion
    }
}