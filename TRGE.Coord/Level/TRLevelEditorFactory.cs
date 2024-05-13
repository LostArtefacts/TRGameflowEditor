using System.Reflection;
using TRGE.Core;

namespace TRGE.Coord;

public static class TRLevelEditorFactory
{
    private static readonly List<EditorType> _editorTypes = new()
    {
        new()
        {
            Version = TRVersion.TR1,
            Type = typeof(TR1LevelEditor),
        },
        new()
        {
            Version = TRVersion.TR2,
            Type = typeof(TR1LevelEditor),
        },
        new()
        {
            Version = TRVersion.TR3,
            Type = typeof(TR1LevelEditor),
        }
    };

    public static void RegisterEditor(TREdition edition, Type type)
        => RegisterEditor(edition.Version, edition.Remastered, type);

    public static void RegisterEditor(TRVersion version, bool remastered, Type type)
    {
        DeregisterEditor(version, remastered);
        _editorTypes.Add(new()
        {
            Version = version,
            Remastered = remastered,
            Type = type
        });
    }

    public static void DeregisterEditor(TREdition edition)
        => DeregisterEditor(edition.Version, edition.Remastered);

    public static void DeregisterEditor(TRVersion version, bool remastered)
    {
        _editorTypes.RemoveAll(t => t.Version == version && t.Remastered == remastered);
    }

    internal static AbstractTRLevelEditor GetLevelEditor(TRDirectoryIOArgs io, TREdition edition)
    {
        EditorType type = GetEditorType(edition);
        if (type == null)
        {
            return null;
        }

        return (AbstractTRLevelEditor)Activator.CreateInstance
        (
            type.Type,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new object[] { io, edition },
            null
        );
    }

    internal static bool EditionSupportsLevelEditing(TREdition edition)
         =>  GetEditorType(edition) != null;

    internal static EditorType GetEditorType(TREdition edition)
    {
        return GetEditorType(edition.Version, edition.Remastered);
    }

    internal static EditorType GetEditorType(TRVersion version, bool remastered)
    {
        return _editorTypes.Find(t => t.Version == version && t.Remastered == remastered);
    }
}

internal class EditorType
{
    public TRVersion Version { get; set; }
    public bool Remastered { get; set; }
    public Type Type { get; set; }
}
