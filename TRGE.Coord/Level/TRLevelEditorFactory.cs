using System.Reflection;
using TRGE.Core;

namespace TRGE.Coord;

public static class TRLevelEditorFactory
{
    private static readonly IReadOnlyDictionary<TRVersion, Type> _defaultTypeMap = new Dictionary<TRVersion, Type>
    {
        [TRVersion.TR1] = typeof(TR1LevelEditor),
        [TRVersion.TR2] = typeof(TR2LevelEditor),
        [TRVersion.TR2G] = typeof(TR2LevelEditor),
        [TRVersion.TR3] = typeof(TR3LevelEditor),
        [TRVersion.TR3G] = typeof(TR3LevelEditor)
    };

    private static readonly Dictionary<TRVersion, Type> _typeMap = _defaultTypeMap.ToDictionary();

    public static void RegisterEditor(TRVersion version, Type type)
    {
        if (_typeMap.ContainsKey(version))
        {
            _typeMap[version] = type;
        }
        else
        {
            _typeMap.Add(version, type);
        }
    }

    public static void DeregisterEditor(TRVersion version, Type type)
    {
        if (_typeMap.ContainsKey(version) && _typeMap[version] == type && _defaultTypeMap.ContainsKey(version))
        {
            _typeMap[version] = _defaultTypeMap[version];
        }
    }

    internal static AbstractTRLevelEditor GetLevelEditor(TRDirectoryIOArgs io, TREdition edition)
    {
        if (EditionSupportsLevelEditing(edition))
        {
            return (AbstractTRLevelEditor)Activator.CreateInstance
            (
                _typeMap[edition.Version],
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
                null,
                new object[] { io, edition },
                null
            );
        }

        return null;
    }

    internal static bool EditionSupportsLevelEditing(TREdition edition)
    {
        return _typeMap.ContainsKey(edition.Version);
    }
}