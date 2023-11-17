using System.Drawing;
using TRGE.Coord;
using TRLevelControl.Model;
using TRTexture16Importer;

namespace WeaponImport
{
    class Program
    {
        static void Main(string[] args)
        {
            TRTexImage8 img8 = new() { Pixels = new byte[256 * 256] };
            TRTexImage16 img16 = new()
            {
                Pixels = TextureUtilities.ImportFromBitmap(new Bitmap(@"Guns.png"))
            };

            SpriteDefinition.WriteWeaponDefinitions(img8, img16, @"Weapons.gz");
        }
    }
}