using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRGE.Coord;

namespace TRGE.Core.Test;

[TestClass]
public class TR2PCItemTests : AbstractTR2ItemTestCollection
{
    protected override int ScriptFileIndex => 0;

    internal override Dictionary<string, List<TRItem>> ManualBonusData => new()
    {
        { 
            AbstractTRScriptedLevel.CreateID(@"data\wall.TR2"), new List<TRItem>
            {
                ExpectedItems[2], ExpectedItems[15]
            }
        },
        {
            AbstractTRScriptedLevel.CreateID(@"data\boat.TR2"), new List<TRItem>
            {
                ExpectedItems[6], ExpectedItems[13], ExpectedItems[14]
            }
        }
    };

    [TestMethod]
    protected void TestRandomiseItemsOutput()
    {
        TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
        sm.SecretBonusOrganisation = Organisation.Random;
        sm.SecretBonusRNG = new RandomGenerator(RandomGenerator.Type.UnixTime);
        int r = sm.SecretBonusRNG.Value;
        sm.SecretBonusRNG.RNGType = RandomGenerator.Type.Custom;
        sm.SecretBonusRNG.Value = r;

        List<string> output = new()
        {
            "Index,Level,Item Type,Item,Quantity"
        };

        for (int i = 0; i < 10; i++)
        {
            if (i > 0)
            {
                ++sm.SecretBonusRNG.Value;
            }
            sm.RandomiseBonuses();

            foreach (MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>> levelBonusData in sm.LevelSecretBonusData)
            {
                foreach (MutableTuple<ushort, TRItemCategory, string, int> bonusItem in levelBonusData.Item3)
                {
                    if (bonusItem.Item4 > 0)
                    {
                        output.Add(i + "," + levelBonusData.Item2 + "," + bonusItem.Item2 + "," + bonusItem.Item3 + "," + bonusItem.Item4);
                    }
                }
            }
        }

        File.WriteAllLines("TR2BonusRandomisation.csv", output.ToArray());
    }

    [TestMethod]
    protected void TestRandomiseItemsShotgun()
    {
        TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
        sm.LevelSequencingOrganisation = Organisation.Random;
        sm.LevelSequencingRNG = new RandomGenerator(RandomGenerator.Type.Date);
        sm.UnarmedLevelOrganisation = Organisation.Random;
        sm.UnarmedLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
        sm.SecretBonusOrganisation = Organisation.Random;
        sm.SecretBonusRNG = new RandomGenerator(RandomGenerator.Type.Date);

        sm.RandomiseLevels();
        sm.RandomiseUnarmedLevels();
        sm.RandomiseBonuses();

        TRItem shotgun = (sm.LevelManager.ItemProvider as TR2ItemProvider).Shotgun;
        for (int i = 0; i < sm.LevelManager.Levels.Count; i++)
        {
            AbstractTRScriptedLevel level = sm.LevelManager.Levels[i];
            if ((level as TR2ScriptedLevel).GetBonusItems(sm.LevelManager.ItemProvider as TR2ItemProvider).Contains(shotgun))
            {
                //should only appear if this level or a previous level removes weapons
                bool weaponsRemoved = false;
                for (int j = i; j >= 0; j--)
                {
                    weaponsRemoved |= sm.LevelManager.Levels[j].RemovesWeapons;
                }

                if (!weaponsRemoved)
                {
                    Assert.Fail("The shotgun was found as a secret bonus item before a weaponless level.");
                }
            }
        }
    }

    [TestMethod]
    protected void TestEnablingSecrets()
    {
        TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
        TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;

        sm.LevelSecretSupportOrganisation = Organisation.Manual;
        List<MutableTuple<string, string, bool>> secretSupport = sm.LevelSecretSupport;
        foreach (MutableTuple<string, string, bool> levelInfo in secretSupport)
        {
            levelInfo.Item3 = true;
        }
        sm.LevelSecretSupport = secretSupport;

        sm.SecretBonusOrganisation = Organisation.Random;
        sm.SecretBonusRNG = new RandomGenerator(RandomGenerator.Type.UnixTime);

        sm.LevelSelectEnabled = true;
        sm.LevelsHaveStartAnimation = false;
        foreach (var level in sm.LevelManager.Levels)
        {
            level.HasSunset = true;
        }
        editor.Save();

        editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
        sm = editor.ScriptEditor as TR23ScriptEditor;
        Assert.IsTrue(sm.LevelManager.GetLevel(AbstractTRScriptedLevel.CreateID("HOUSE")).HasSecrets);
    }

    [TestMethod]
    protected void TestEnablingSunsets()
    {
        TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
        TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;

        sm.LevelSunsetOrganisation = Organisation.Manual;
        List<MutableTuple<string, string, bool>> sunsetData = sm.LevelSunsetData;
        foreach (MutableTuple<string, string, bool> levelInfo in sunsetData)
        {
            levelInfo.Item3 = true;
        }
        sm.LevelSunsetData = sunsetData;

        editor.Save();

        editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
        sm = editor.ScriptEditor as TR23ScriptEditor;

        sunsetData = sm.LevelSunsetData;
        foreach (MutableTuple<string, string, bool> levelInfo in sunsetData)
        {
            Assert.IsTrue(levelInfo.Item3, string.Format("{0} is missing the sunset flag", levelInfo.Item2));
        }
    }
}