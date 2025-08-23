using TRGE.Coord;
using TRGE.Core;
using TRImageControl;
using TRLevelControl.Model;

namespace WeaponImport;

class Program
{
    static void Main()
    {
        var editor = TRCoord.Instance.Open(@"D:\Games\Rando\TR1X 4.14_temp\data", TRScriptOpenOption.DiscardBackup);
        var script = (editor.ScriptEditor.Script as TR1Script);

        script.Write(@"D:\Games\Rando\TR1X 4.14_temp\cfg\tr1\gameflow.json5");
        return;

        TRTexImage8 img8 = new() { Pixels = new byte[256 * 256] };
        TRTexImage16 img16 = new()
        {
            Pixels = new TRImage("Guns.png").ToRGB555()
        };

        SpriteDefinition.WriteWeaponDefinitions(img8, img16, "Weapons.gz");
    }
}
