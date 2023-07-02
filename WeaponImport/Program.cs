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
            TRTexImage8 img8 = new TRTexImage8 { Pixels = new byte[256 * 256] };
            TRTexImage16 img16 = new TRTexImage16
            {
                Pixels = TextureUtilities.ImportFromBitmap(new Bitmap(@"Guns.png"))
            };

            SpriteDefinition.WriteWeaponDefinitions(img8, img16, @"Weapons.gz");
        }
    }
}