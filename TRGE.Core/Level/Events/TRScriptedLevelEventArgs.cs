using System;
using System.Collections.Generic;

namespace TRGE.Core
{
    public class TRScriptedLevelEventArgs : EventArgs
    {
        public TRScriptedLevelModification Modification { get; private set; }
        internal AbstractTRScriptedLevel ScriptedLevel { get; private set; }
        public string LevelID { get; internal set; }
        public string LevelName { get; internal set; }
        public string LevelFileBaseName { get; internal set; }
        public ushort LevelSequence { get; internal set; }
        public bool IsFinalLevel { get; internal set; }
        public bool LevelRemovesWeapons { get; internal set; }
        public bool LevelRemovesAmmo { get; internal set; }
        public bool LevelHasSunset { get; internal set; }

        private TRScriptedLevelEventArgs() { }

        internal static TRScriptedLevelEventArgs Create(AbstractTRScriptedLevel level, TRScriptedLevelModification modification)
        {
            return new TRScriptedLevelEventArgs
            {
                Modification = modification,
                ScriptedLevel = level,
                LevelID = level.ID,
                LevelName = level.Name,
                LevelFileBaseName = level.LevelFileBaseName,
                LevelSequence = level.Sequence,
                IsFinalLevel = level.IsFinalLevel,
                LevelRemovesWeapons = level.RemovesWeapons,
                LevelRemovesAmmo = level.RemovesAmmo,
                LevelHasSunset = level.HasSunset
            };
        }

        public override bool Equals(object obj)
        {
            return obj is TRScriptedLevelEventArgs args &&
                   Modification == args.Modification &&
                   LevelID == args.LevelID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Modification, LevelID);
        }
    }
}