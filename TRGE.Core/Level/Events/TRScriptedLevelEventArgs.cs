using System;

namespace TRGE.Core
{
    public class TRScriptedLevelEventArgs : EventArgs
    {
        public string LevelID { get; internal set; }
        public string LevelName { get; internal set; }
        public string LevelFileBaseName { get; internal set; }
        public ushort LevelSequence { get; internal set; }
        public bool IsFinalLevel { get; internal set; }
        public bool LevelRemovesWeapons { get; internal set; }
        public bool LevelRemovesAmmo { get; internal set; }

        internal static TRScriptedLevelEventArgs Create(AbstractTRScriptedLevel level)
        {
            return new TRScriptedLevelEventArgs
            {
                LevelID = level.ID,
                LevelName = level.Name,
                LevelFileBaseName = level.LevelFileBaseName,
                LevelSequence = level.Sequence,
                IsFinalLevel = level.IsFinalLevel,
                LevelRemovesWeapons = level.RemovesWeapons,
                LevelRemovesAmmo = level.RemovesAmmo
            };
        }
    }
}