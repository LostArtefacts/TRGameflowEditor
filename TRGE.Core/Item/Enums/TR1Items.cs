﻿namespace TRGE.Core.Item.Enums;

public enum TR1Items
{
    Lara                   = 0,
    LaraPistolAnim_H       = 1,
    LaraShotgunAnim_H      = 2,
    LaraMagnumAnim_H       = 3,
    LaraUziAnimation_H     = 4,
    LaraMiscAnim_H         = 5,
    Doppelganger           = 6,
    Wolf                   = 7,
    Bear                   = 8,
    Bat                    = 9,
    CrocodileLand          = 10,
    CrocodileWater         = 11,
    Lion                   = 12,
    Lioness                = 13,
    Panther                = 14,
    Gorilla                = 15,
    RatLand                = 16,
    RatWater               = 17,
    TRex                   = 18,
    Raptor                 = 19,
    FlyingAtlantean        = 20,
    ShootingAtlantean_N    = 21, // Requires FlyingAtlantean
    NonShootingAtlantean_N = 22, // Requires FlyingAtlantean
    Centaur                = 23,
    Mummy                  = 24, // Qualopec only, Egypt = NonShootingAtlantean_N
    DinoWarrior_U          = 25,
    Fish_U                 = 26,
    Larson                 = 27,
    Pierre                 = 28,
    Skateboard             = 29,
    SkateboardKid          = 30,
    Cowboy                 = 31,
    Kold                   = 32,
    Natla                  = 33,
    Adam                   = 34,
    FallingBlock           = 35,
    SwingingBlade          = 36,
    TeethSpikes            = 37,
    RollingBall            = 38,
    Dart_H                 = 39,
    DartEmitter            = 40,
    LiftingDoor            = 41,
    SlammingDoor           = 42,
    DamoclesSword          = 43,
    ThorHammerHandle       = 44,
    ThorHammerBlock        = 45,
    ThorLightning          = 46,
    Barricade              = 47,
    PushBlock1             = 48,
    PushBlock2             = 49,
    PushBlock3             = 50,
    PushBlock4             = 51,
    MovingBlock            = 52,
    FallingCeiling1        = 53,
    FallingCeiling2        = 54,
    WallSwitch             = 55,
    UnderwaterSwitch       = 56,
    Door1                  = 57,
    Door2                  = 58,
    Door3                  = 59,
    Door4                  = 60,
    Door5                  = 61,
    Door6                  = 62,
    Door7                  = 63,
    Door8                  = 64,
    Trapdoor1              = 65,
    Trapdoor2              = 66,
    Trapdoor3              = 67,
    BridgeFlat             = 68,
    BridgeTilt1            = 69,
    BridgeTilt2            = 70,
    PassportOpen_M_H       = 71,
    Compass_M_H            = 72,
    LaraHomePhoto_M_H      = 73,
    Animating1             = 74,
    Animating2             = 75,
    Animating3             = 76,
    CutsceneActor1         = 77,
    CutsceneActor2         = 78,
    CutsceneActor3         = 79,
    CutsceneActor4         = 80,
    PassportClosed_M_H     = 81,
    Map_M_U                = 82,
    SavegameCrystal_P      = 83,
    Pistols_S_P            = 84,
    Shotgun_S_P            = 85,
    Magnums_S_P            = 86,
    Uzis_S_P               = 87,
    PistolAmmo_S_P         = 88,
    ShotgunAmmo_S_P        = 89,
    MagnumAmmo_S_P         = 90,
    UziAmmo_S_P            = 91,
    Explosive_S_U          = 92,
    SmallMed_S_P           = 93,
    LargeMed_S_P           = 94,
    Sunglasses_M_H         = 95,
    CassettePlayer_M_H     = 96,
    DirectionKeys_M_H      = 97,
    Flashlight_U           = 98,
    Pistols_M_H            = 99,
    Shotgun_M_H            = 100,
    Magnums_M_H            = 101,
    Uzis_M_H               = 102,
    PistolAmmo_M_H         = 103,
    ShotgunAmmo_M_H        = 104,
    MagnumAmmo_M_H         = 105,
    UziAmmo_M_H            = 106,
    Explosive_M_H_U        = 107,
    SmallMed_M_H           = 108,
    LargeMed_M_H           = 109,
    Puzzle1_S_P            = 110,
    Puzzle2_S_P            = 111,
    Puzzle3_S_P            = 112,
    Puzzle4_S_P            = 113,
    Puzzle1_M_H            = 114,
    Puzzle2_M_H            = 115,
    Puzzle3_M_H            = 116,
    Puzzle4_M_H            = 117,
    PuzzleHole1            = 118,
    PuzzleHole2            = 119,
    PuzzleHole3            = 120,
    PuzzleHole4            = 121,
    PuzzleDone1            = 122,
    PuzzleDone2            = 123,
    PuzzleDone3            = 124,
    PuzzleDone4            = 125,
    LeadBar_S_P            = 126,
    LeadBar_M_H            = 127,
    MidasHand_N            = 128,
    Key1_S_P               = 129,
    Key2_S_P               = 130,
    Key3_S_P               = 131,
    Key4_S_P               = 132,
    Key1_M_H               = 133,
    Key2_M_H               = 134,
    Key3_M_H               = 135,
    Key4_M_H               = 136,
    Keyhole1               = 137,
    Keyhole2               = 138,
    Keyhole3               = 139,
    Keyhole4               = 140,
    Quest1_S_P             = 141,
    Quest2_S_P             = 142,
    ScionPiece1_S_P        = 143, // ToQ and Sanctuary - triggers LaraMiscAnim
    ScionPiece2_S_P        = 144, // From Pierre - normal pickup
    ScionPiece3_S_P        = 145, // Great Pyramid - targetable
    ScionPiece4_S_P        = 146, // Atlantis - triggers LaraMiscAnim
    ScionHolder            = 147,
    Quest1_M_H             = 148,
    Quest2_M_H             = 149,
    ScionPiece_M_H         = 150,
    Explosion1_S_H         = 151,
    Explosion2_S_H         = 152,
    WaterRipples1_S_H      = 153,
    WaterRipples2_S_H      = 154,
    Bubbles1_S_H           = 155,
    Bubbles2_S_H           = 156,
    BubbleEmitter_N        = 157,
    Blood1_S_H             = 158,
    Blood2_S_H             = 159,
    DartEffect_S_H         = 160,
    CentaurStatue          = 161,
    NatlasMineShack        = 162,
    AtlanteanEgg           = 163,
    Ricochet_S_H           = 164,
    Sparkles_S_H           = 165,
    Gunflare_H             = 166,
    Dust_S_H               = 167,
    BodyPart_N             = 168,
    CameraTarget_N         = 169,
    WaterfallMist_N        = 170,
    Missile1_H             = 171, // Natla
    Missile2_H             = 172, // Meatball
    Missile3_H             = 173, // Bone
    Missile4_U             = 174,
    Missile5_U             = 175,
    LavaParticles_S_H      = 176,
    LavaEmitter_N          = 177,
    Flame_S_H              = 178,
    FlameEmitter_N         = 179,
    AtlanteanLava          = 180,
    AdamEgg                = 181,
    Motorboat              = 182,
    Earthquake_N           = 183,

    Unused01               = 184,
    Unused02               = 185,
    Unused03               = 186,
    Unused04               = 187,
    Unused05               = 188,
    
    LaraPonytail_H_U       = 189,
    FontGraphics_S_H       = 190,
}
