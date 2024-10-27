using TRGE.Coord;
using TRImageControl;
using TRLevelControl.Model;

namespace WeaponImport;

class Program
{
    static void Main()
    {
        TRTexImage8 img8 = new() { Pixels = new byte[256 * 256] };
        TRTexImage16 img16 = new()
        {
            Pixels = new TRImage("Guns.png").ToRGB555()
        };

        SpriteDefinition.WriteWeaponDefinitions(img8, img16, "Weapons.gz");
    }
}
