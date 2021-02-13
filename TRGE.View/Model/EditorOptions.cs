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
        private bool _titleEnabled, _levelSelectEnabled, _saveLoadEnabled, _optionRingEnabled;
        private bool _fmvsEnabled, _cutscenesEnabled, _startAnimationsEnabled, _cheatsEnabled, _dozyViable, _dozyEnabled;
        private bool _demosEnabled, _trainingEnabled;
        private int _demoDelay;
        
        private bool _useDefaultLevelSequence, _useManualLevelSequence, _useRandomLevelSequence;
        private List<Tuple<string, string>> _levelSequencing;

        private bool _useDefaultUnarmedLevels, _useManualUnarmedLevels;
        private List<MutableTuple<string, string, bool>> _unarmedLevelData;

        private bool _useDefaultAmmolessLevels, _useManualAmmolessLevels;
        private List<MutableTuple<string, string, bool>> _ammolessLevelData;

        private bool _secretRewardsViable;
        private bool _useDefaultSecretBonuses, _useManualSecretBonuses;
        private List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> _secretBonusData;

        private bool _sunsetsViable;
        private bool _useDefaultSunsets, _useManualSunsets;
        private List<MutableTuple<string, string, bool>> _sunsetLevelData;

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

        public bool UseRandomLevelSequence
        {
            get => _useRandomLevelSequence;
            set
            {
                _useRandomLevelSequence = value;
                OnPropertyChanged();
            }
        }

        public LevelSequencingData LevelSequencing
        {
            get => new LevelSequencingData(_levelSequencing);
            set
            {
                _levelSequencing = value.ToTupleList();
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

        private TR23ScriptEditor _editor;

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

            DemosEnabled = editor.DemosEnabled;
            DemoDelay = (int)editor.DemoTime;
            TrainingEnabled = editor.GymEnabled;

            UseManualLevelSequence = false;// editor.LevelSequencingOrganisation == Organisation.Manual;
            UseDefaultLevelSequence = false;// editor.LevelSequencingOrganisation == Organisation.Default;
            UseRandomLevelSequence = true;// editor.LevelSequencingOrganisation == Organisation.Random;
            _levelSequencing = editor.LevelSequencing;

            UseManualUnarmed = editor.UnarmedLevelOrganisation == Organisation.Manual;
            UseDefaultUnarmed = !UseManualUnarmed;
            _unarmedLevelData = editor.UnarmedLevelData;

            UseManualAmmoless = editor.AmmolessLevelOrganisation == Organisation.Manual;
            UseDefaultAmmoless = !UseManualAmmoless;
            _ammolessLevelData = editor.AmmolessLevelData;

            SecretRewardsViable = editor.CanOrganiseBonuses;
            UseManualBonuses = editor.SecretBonusOrganisation == Organisation.Manual;
            UseDefaultBonuses = !UseManualBonuses;
            _secretBonusData = editor.LevelSecretBonusData;

            SunsetsViable = editor.CanSetSunsets;
            UseManualSunsets = editor.LevelSunsetOrganisation == Organisation.Manual;
            UseDefaultSunsets = !UseManualSunsets;
            _sunsetLevelData = editor.LevelSunsetData;

            UseManualAudio = editor.GameTrackOrganisation == Organisation.Manual;
            UseDefaultAudio = !UseManualAudio;
            _audioData = editor.GameTrackData;

            _allAudioTracks = editor.AllGameTracks;
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

            _editor.LevelSequencingOrganisation = UseManualLevelSequence ? Organisation.Manual : Organisation.Default;
            if (UseManualLevelSequence)
            {
                _editor.LevelSequencing = _levelSequencing;
            }

            _editor.UnarmedLevelOrganisation = UseManualUnarmed ? Organisation.Manual : Organisation.Default;
            if (UseManualUnarmed)
            {
                _editor.UnarmedLevelData = _unarmedLevelData;
            }

            _editor.AmmolessLevelOrganisation = UseManualAmmoless ? Organisation.Manual : Organisation.Default;
            if (UseManualAmmoless)
            {
                _editor.AmmolessLevelData = _ammolessLevelData;
            }

            if (SecretRewardsViable)
            {
                _editor.SecretBonusOrganisation = UseManualBonuses ? Organisation.Manual : Organisation.Default;
                if (UseManualBonuses)
                {
                    _editor.LevelSecretBonusData = _secretBonusData;
                }
            }

            if (SunsetsViable)
            {
                _editor.LevelSunsetOrganisation = UseManualSunsets ? Organisation.Manual : Organisation.Default;
                if (UseManualSunsets)
                {
                    _editor.LevelSunsetData = _sunsetLevelData;
                }
            }

            _editor.GameTrackOrganisation = UseManualAudio ? Organisation.Manual : Organisation.Default;
            if (UseManualAudio)
            {
                _editor.GameTrackData = _audioData;
            }
        }

        public byte[] GetAudioTrackData(AudioTrack track)
        {
            return _editor.GetTrackData(track.ID);
        }

        public AudioData GetAudioData()
        {
            return GlobalAudioData;
        }
    }
}