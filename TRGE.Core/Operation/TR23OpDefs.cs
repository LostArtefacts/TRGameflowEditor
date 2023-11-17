namespace TRGE.Core
{
    internal static class TR23OpDefs
    {
        internal static readonly TROpDef Picture        = new(0, true);
        internal static readonly TROpDef ListStart      = new(1, true);
        internal static readonly TROpDef ListEnd        = new(2, true);
        internal static readonly TROpDef FMV            = new(3, true);
        internal static readonly TROpDef Level          = new(4, true);
        internal static readonly TROpDef Cinematic      = new(5, true);
        internal static readonly TROpDef Complete       = new(6, false);
        internal static readonly TROpDef Demo           = new(7, true);
        internal static readonly TROpDef JumpToSequence = new(8, true);
        internal static readonly TROpDef End            = new(9, false);
        internal static readonly TROpDef Track          = new(10, true);
        internal static readonly TROpDef Sunset         = new(11, false, Track);
        internal static readonly TROpDef LoadPic        = new(12, true);
        internal static readonly TROpDef DeadlyWater    = new(13, false);
        internal static readonly TROpDef RemoveWeapons  = new(14, false, Track);
        internal static readonly TROpDef GameComplete   = new(15, false);
        internal static readonly TROpDef CutAngle       = new(16, true);
        internal static readonly TROpDef NoFloor        = new(17, true);
        internal static readonly TROpDef StartInvBonus  = new(18, true, Track);
        internal static readonly TROpDef StartAnimation = new(19, true);
        internal static readonly TROpDef Secrets        = new(20, true, Track);
        internal static readonly TROpDef KillToComplete = new(21, false);
        internal static readonly TROpDef RemoveAmmo     = new(22, false, Track);

        // tomb3
        internal static readonly TROpDef HasRain = new(23, false, Track);
        internal static readonly TROpDef HasSnow = new(24, false, Track);
        internal static readonly TROpDef WaterParts = new(25, false, Track);
        internal static readonly TROpDef IsCold = new(26, false, Track);
        // 0 = lava
        // 1 = rapids
        // 2 = electricity
        internal static readonly TROpDef DeathTile = new(27, true, Track);
        // 2 operands - 32-bit RGB
        internal static readonly TROpDef WaterColour = new(28, true, Track);

        private static readonly TROpDef[] All = new TROpDef[]
        {
            Picture, ListStart, ListEnd, FMV, Level, Cinematic, Complete, Demo, JumpToSequence, End, Track, Sunset, LoadPic, DeadlyWater, 
            RemoveWeapons, GameComplete, CutAngle, NoFloor, StartInvBonus, StartAnimation, Secrets, KillToComplete, RemoveAmmo,
            HasRain, HasSnow, WaterParts, IsCold, DeathTile, WaterColour
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

        internal static string GetName(TROpDef def)
        {
            switch (def.OpCode)
            {
                case 0:
                    return nameof(Picture);
                case 1:
                    return nameof(ListStart);
                case 2:
                    return nameof(ListEnd);
                case 3:
                    return nameof(FMV);
                case 4:
                    return nameof(Level);
                case 5:
                    return nameof(Cinematic);
                case 6:
                    return nameof(Complete);
                case 7:
                    return nameof(Demo);
                case 8:
                    return nameof(JumpToSequence);
                case 9:
                    return nameof(End);
                case 10:
                    return nameof(Track);
                case 11:
                    return nameof(Sunset);
                case 12:
                    return nameof(LoadPic);
                case 13:
                    return nameof(DeadlyWater);
                case 14:
                    return nameof(RemoveWeapons);
                case 15:
                    return nameof(GameComplete);
                case 16:
                    return nameof(CutAngle);
                case 17:
                    return nameof(NoFloor);
                case 18:
                    return nameof(StartInvBonus);
                case 19:
                    return nameof(StartAnimation);
                case 20:
                    return nameof(Secrets);
                case 21:
                    return nameof(KillToComplete);
                case 22:
                    return nameof(RemoveAmmo);
                case 23:
                    return nameof(HasRain);
                case 24:
                    return nameof(HasSnow);
                case 25:
                    return nameof(WaterParts);
                case 26:
                    return nameof(IsCold);
                case 27:
                    return nameof(DeathTile);
                case 28:
                    return nameof(WaterColour);
                default:
                    return "Unknown";
            }
        }
    }
}