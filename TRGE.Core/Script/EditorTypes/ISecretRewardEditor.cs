namespace TRGE.Core;

public interface ISecretRewardEditor
{
    Organisation SecretBonusOrganisation { get; set; }

    RandomGenerator SecretBonusRNG { get; set; }

    uint RandomAmmolessLevelCount { get; set; }

    List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> LevelSecretBonusData { get; set; }
}