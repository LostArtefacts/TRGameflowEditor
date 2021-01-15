namespace TRGE.Core
{
    internal static class TR23OpDefs
    {
        internal static readonly TROpDef Picture        = new TROpDef(0, true);
        internal static readonly TROpDef ListStart      = new TROpDef(1, true);
        internal static readonly TROpDef ListEnd        = new TROpDef(2, true);
        internal static readonly TROpDef FMV            = new TROpDef(3, true);
        internal static readonly TROpDef Level          = new TROpDef(4, true);
        internal static readonly TROpDef Cinematic      = new TROpDef(5, true);
        internal static readonly TROpDef Complete       = new TROpDef(6, false);
        internal static readonly TROpDef Demo           = new TROpDef(7, true);
        internal static readonly TROpDef JumpToSequence = new TROpDef(8, true);
        internal static readonly TROpDef End            = new TROpDef(9, false);
        internal static readonly TROpDef Track          = new TROpDef(10, true);
        internal static readonly TROpDef Sunset         = new TROpDef(11, false);
        internal static readonly TROpDef LoadPic        = new TROpDef(12, true);
        internal static readonly TROpDef DeadlyWater    = new TROpDef(13, false);
        internal static readonly TROpDef RemoveWeapons  = new TROpDef(14, false, LoadPic);
        internal static readonly TROpDef GameComplete   = new TROpDef(15, false);
        internal static readonly TROpDef CutAngle       = new TROpDef(16, true);
        internal static readonly TROpDef NoFloor        = new TROpDef(17, true);
        internal static readonly TROpDef StartInvBonus  = new TROpDef(18, true, Track);
        internal static readonly TROpDef StartAnimation = new TROpDef(19, true);
        internal static readonly TROpDef Secrets        = new TROpDef(20, true);
        internal static readonly TROpDef KillToComplete = new TROpDef(21, false);
        internal static readonly TROpDef RemoveAmmo     = new TROpDef(22, false);

        private static readonly TROpDef[] All = new TROpDef[]
        {
            Picture, ListStart, ListEnd, FMV, Level, Cinematic, Complete, Demo, JumpToSequence, End, Track, Sunset, LoadPic, DeadlyWater, 
            RemoveWeapons, GameComplete, CutAngle, NoFloor, StartInvBonus, StartAnimation, Secrets, KillToComplete, RemoveAmmo
        };

        internal static TROpDef Get(ushort opCode)
        {
            foreach (TROpDef def in All)
            {
                if (def.OpCode == opCode)
                {
                    return def;
                }
            }
            return null;
        }
    }
}