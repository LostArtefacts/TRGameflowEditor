namespace TRGE.Core;

public interface IUnarmedEditor
{
    Organisation UnarmedLevelOrganisation { get; set; }
    RandomGenerator UnarmedLevelRNG { get; set; }
    uint RandomUnarmedLevelCount { get; set; }

    List<MutableTuple<string, string, bool>> UnarmedLevelData { get; set; }
}