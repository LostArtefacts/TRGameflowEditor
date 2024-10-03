using System.Reflection.Emit;

namespace TRGE.Core;

public class TRRScriptedLevel : AbstractTRScriptedLevel
{
    private static readonly Dictionary<TRVersion, Dictionary<ushort, ushort>> _secretCount = new()
    {
        [TRVersion.TR1] = new()
        {
            // Gym
            [0] = 0,
            // Peru
            [1] = 3,
            [2] = 3,
            [3] = 5,
            [4] = 3,
            // Greece
            [5] = 4,
            [6] = 3,
            [7] = 3,
            [8] = 3,
            [9] = 2,
            // Egypt
            [10] = 3,
            [11] = 3,
            [12] = 1,
            // Atlantis
            [13] = 3,
            [14] = 3,
            [15] = 3,
        },
        [TRVersion.TR2] = new()
        {
            // Gym
            [0] = 0,
            // Italy
            [1] = 3,
            [2] = 3,
            [3] = 3,
            [4] = 3,
            // Offshore
            [5] = 3,
            [6] = 3,
            [7] = 3,
            [8] = 3,
            [9] = 3,
            [10] = 3,
            // Tibet
            [11] = 3,
            [12] = 3,
            [13] = 3,
            [14] = 3,
            // China
            [15] = 3,
            [16] = 3,
            [17] = 0,
            [18] = 0,
        },
        [TRVersion.TR3] = new()
        {
            [0] = 0,
            // India
            [1] = 6,
            [2] = 4,
            [3] = 5,
            [4] = 0,
            // South Pacific
            [5] = 4,
            [6] = 3,
            [7] = 3,
            [8] = 1,
            // London
            [9] = 5,
            [10] = 5,
            [11] = 6,
            [12] = 1,
            // Nevada
            [13] = 3,
            [14] = 2,
            [15] = 3,
            // Antarctica
            [16] = 3,
            [17] = 3,
            [18] = 3,
            [19] = 0,
            // All Hallows
            [20] = 0,
        }
    };

    private static readonly string _mapExt = ".MAP";
    private static readonly string _pdpExt = ".PDP";
    private static readonly string _texExt = ".TEX";
    private static readonly string _trgExt = ".TRG";

    public TRVersion Version { get; private set; }

    public TRRScriptedLevel(TRVersion version)
    {
        Version = version;
        for (int i = 0; i < 4; i++)
        {
            Keys.Add(null);
            Puzzles.Add(null);
            if (i < 2)
            {
                Pickups.Add(null);
            }
        }
    }

    private ushort _sequence;
    public override ushort Sequence
    {
        get => _sequence;
        set
        {
            _sequence = value;
        }
    }

    public string MapFile => Path.ChangeExtension(LevelFile, _mapExt);
    public string PdpFile => Path.ChangeExtension(LevelFile, _pdpExt);
    public string TexFile => Path.ChangeExtension(LevelFile, _texExt);
    public string TrgFile => Path.ChangeExtension(LevelFile, _trgExt);
    public string MapFileBaseName => Path.GetFileName(MapFile);
    public string PdpFileBaseName => Path.GetFileName(PdpFile);
    public string TexFileBaseName => Path.GetFileName(TexFile);
    public string TrgFileBaseName => Path.GetFileName(TrgFile);

    public List<string> AllFiles
    {
        get
        {
            List<string> files = new()
            {
                LevelFile, PdpFile, TexFile, TrgFile
            };
            if (!IgnoreMap)
            {
                files.Add(MapFile);
            }
            return files;
        }
    }

    public bool IgnoreMap {  get; set; }
    public bool HasColdWater { get; set; }

    #region Legacy
    public override ushort TrackID { get; set; }
    public override bool HasFMV { get; set; }

    public override bool SupportsFMVs { get; }

    public override bool HasStartAnimation { get; set; }

    public override bool SupportsStartAnimations { get; }

    public override short StartAnimationID { get; set; }
    public override bool HasCutScene
    {
        get => CutSceneLevel != null;
        set { }
    }

    public override bool SupportsCutScenes { get; }

    public override AbstractTRScriptedLevel CutSceneLevel { get; set; }
    public override bool HasSunset { get; set; }
    public override bool HasDeadlyWater { get; set; }
    public override bool RemovesWeapons { get; set; }
    public override bool RemovesAmmo { get; set; }
    public override bool HasSecrets { get; set; }
    public override bool KillToComplete { get; set; }
    public override bool IsFinalLevel { get; set; }
    public bool IsCutscene { get; set; }

    public override ushort NumSecrets
    {
        get => _secretCount[Version][Sequence];
        set { }
    }

    protected override TROpDef GetOpDefFor(ushort scriptData)
    {
        throw new NotSupportedException();
    }
    #endregion
}
