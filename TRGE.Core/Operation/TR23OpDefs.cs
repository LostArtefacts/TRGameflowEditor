namespace TRGE.Core;

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
        return def.OpCode switch
        {
            0 => nameof(Picture),
            1 => nameof(ListStart),
            2 => nameof(ListEnd),
            3 => nameof(FMV),
            4 => nameof(Level),
            5 => nameof(Cinematic),
            6 => nameof(Complete),
            7 => nameof(Demo),
            8 => nameof(JumpToSequence),
            9 => nameof(End),
            10 => nameof(Track),
            11 => nameof(Sunset),
            12 => nameof(LoadPic),
            13 => nameof(DeadlyWater),
            14 => nameof(RemoveWeapons),
            15 => nameof(GameComplete),
            16 => nameof(CutAngle),
            17 => nameof(NoFloor),
            18 => nameof(StartInvBonus),
            19 => nameof(StartAnimation),
            20 => nameof(Secrets),
            21 => nameof(KillToComplete),
            22 => nameof(RemoveAmmo),
            23 => nameof(HasRain),
            24 => nameof(HasSnow),
            25 => nameof(WaterParts),
            26 => nameof(IsCold),
            27 => nameof(DeathTile),
            28 => nameof(WaterColour),
            _ => "Unknown",
        };
    }
}