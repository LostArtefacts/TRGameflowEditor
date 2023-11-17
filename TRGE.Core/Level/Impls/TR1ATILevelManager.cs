namespace TRGE.Core
{
    internal class TR1ATILevelManager : AbstractTRLevelManager
    {
        private readonly TR1AudioProvider _audioProvider;
        private readonly TR1ATIScript _script;
        private readonly TR1ScriptedLevel _assaultLevel;

        internal override int LevelCount => _script.Levels.Count;
        public override AbstractTRAudioProvider AudioProvider => _audioProvider;
        internal override AbstractTRItemProvider ItemProvider => throw new NotSupportedException();

        internal override AbstractTRScriptedLevel AssaultLevel => _assaultLevel;
        internal override List<AbstractTRScriptedLevel> Levels
        {
            get => _script.Levels;
            set => _script.Levels = value;
        }

        protected override ushort TitleSoundID
        {
            get => _script.TitleSoundID;
            set => _script.TitleSoundID = value;
        }

        protected override ushort SecretSoundID
        {
            get => _script.SecretSoundID;
            set => _script.SecretSoundID = value;
        }

        internal TR1ATILevelManager(TR1ATIScript script)
            : base(script.Edition)
        {
            _script = script;
            _assaultLevel = _script.AssaultLevel as TR1ScriptedLevel;
            _audioProvider = TRAudioFactory.GetAudioProvider(script.Edition) as TR1AudioProvider;
        }

        protected override TRScriptedLevelModification OpDefToModification(TROpDef opDef)
        {
            throw new NotImplementedException();
        }

        internal override void Save() { }

        internal override void UpdateScript() { }
    }
}