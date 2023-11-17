using System.Text;

namespace TRGE.Core;

public abstract class AbstractTRScriptedLevel : AbstractTROperationContainer
{
    public string ID { get; private set; }
    public bool Enabled { get; set; }
    public string Name { get; set; }

    protected string _levelFile;
    public string LevelFile
    {
        get => _levelFile;
        internal set
        {
            ID = CreateID(_levelFile = value);
        }
    }

    public string LevelFileBaseName => Path.GetFileName(LevelFile);

    protected List<string> _puzzles, _keys, _pickups;
    public List<string> Puzzles => _puzzles;
    public List<string> Keys => _keys;
    public List<string> Pickups => _pickups;

    public abstract ushort Sequence { get; set; }
    public ushort OriginalSequence { get; internal set; }
    public abstract ushort TrackID { get; set; }
    public abstract bool HasFMV { get; set; }
    public abstract bool SupportsFMVs { get; }
    public abstract bool HasStartAnimation { get; set; }
    public abstract bool SupportsStartAnimations { get; }
    public abstract short StartAnimationID { get; set; }
    public abstract bool HasCutScene { get; set; }
    public abstract bool SupportsCutScenes { get; }
    public abstract AbstractTRScriptedLevel CutSceneLevel { get; set; }
    public abstract bool HasSunset { get; set; }
    public abstract bool HasDeadlyWater { get; set; }
    public abstract bool RemovesWeapons { get; set; }
    public abstract bool RemovesAmmo { get; set; }
    public abstract bool HasSecrets { get; set; }
    public abstract ushort NumSecrets { get; set; }
    public abstract bool KillToComplete { get; set; }
    public abstract bool IsFinalLevel { get; set; }

    public bool OptionallyRemovesWeapons => RemovesWeapons && !RemovesAmmo;
    public bool ForciblyRemovesWeapons => RemovesWeapons && RemovesAmmo;

    internal AbstractTRScriptedLevel()
    {
        _puzzles = new List<string>();
        _keys = new List<string>();
        _pickups = new List<string>();
        Enabled = true;
    }

    public virtual void SetDefaults() { }

    internal void AddPuzzle(string puzzle)
    {
        _puzzles.Add(puzzle);
    }

    internal void AddKey(string key)
    {
        _keys.Add(key);
    }

    internal void AddPickup(string pickup)
    {
        _pickups.Add(pickup);
    }

    internal void CopyOperation(TROpDef opDef, AbstractTRScriptedLevel other)
    {
        if (!HasOperation(opDef))
        {
            int i = other.GetOperationIndex(opDef);
            if (i != -1)
            {
                _operations.Insert(i, other.GetOperation(opDef));
            }
        }
    }

    internal static string CreateID(string identifier)
    {
        return Path.GetFileNameWithoutExtension(identifier).ToUpper().CreateMD5();
    }

    /// <summary>
    /// Checks if the scripted level's file name matches the supplied argument.
    /// </summary>
    public bool Is(string levelFileName)
    {
        return CreateID(levelFileName).Equals(ID);
    }

    public override bool Equals(object obj)
    {
        return obj is AbstractTRScriptedLevel && (obj as AbstractTRScriptedLevel).ID == ID;
    }

    public override int GetHashCode()
    {
        return 1213502048 + EqualityComparer<string>.Default.GetHashCode(ID);
    }

    public virtual void SerializeToMain(BinaryWriter writer)
    {
        writer.Write((byte)OriginalSequence);
        writer.Write((byte)Sequence);
    }

    public override string ToString()
    {
        StringBuilder sb = new(base.ToString());

        sb.Append(" Name: " + Name);
        sb.Append(", File: " + LevelFileBaseName.ToUpper());
        sb.Append(", Sequence: " + Sequence);

        return sb.ToString();
    }
}