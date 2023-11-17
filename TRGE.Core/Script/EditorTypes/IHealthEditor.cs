namespace TRGE.Core
{
    public interface IHealthEditor
    {
        Organisation MedilessLevelOrganisation { get; set; }

        RandomGenerator MedilessLevelRNG { get; set; }

        uint RandomMedilessLevelCount { get; set; }

        List<MutableTuple<string, string, bool>> MedilessLevelData { get; set; }
        bool DisableHealingBetweenLevels { get; set; }
        bool DisableMedpacks { get; set; }
    }
}