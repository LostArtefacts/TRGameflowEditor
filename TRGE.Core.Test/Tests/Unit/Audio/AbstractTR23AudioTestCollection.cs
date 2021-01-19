using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace TRGE.Core.Test
{
    public abstract class AbstractTR23AudioTestCollection : BaseTestCollection
    {
        protected abstract int ScriptFileIndex { get; }
        protected abstract ushort SampleTrack { get; }
        internal abstract Dictionary<string, ushort> ExpectedLevelTracks { get; }
        internal abstract Dictionary<string, ushort> NewLevelTracks { get; }

        protected override void Setup()
        {
            base.Setup();
            File.Copy(@"audio\tr2audio.wad", Path.Combine(TRGameflowEditor.Instance.GetConfigDirectory(), "tr2audio.wad"));
            File.Copy(@"audio\tr3audio.wad", Path.Combine(TRGameflowEditor.Instance.GetConfigDirectory(), "tr3audio.wad"));
        }

        [TestMethod]
        [TestSequence(0)]
        protected void TestLoadTracks()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                List<MutableTuple<string, string, ushort>> trackData = sm.GameTrackData;
                CompareTrackData(trackData, ExpectedLevelTracks);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseAllScriptManagers();
            }
        }

        //[TestMethod]
        [TestSequence(1)]
        protected void TestPlayTrack()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                using (MemoryStream ms = new MemoryStream(sm.GetTrackData(SampleTrack)))
                {
                    new SoundPlayer(ms).PlaySync();
                }
            }
            finally
            {
                TRGameflowEditor.Instance.CloseAllScriptManagers();
            }
        }

        [TestMethod]
        [TestSequence(2)]
        protected void TestSetTracks()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                List<MutableTuple<string, string, ushort>> trackData = sm.GameTrackData;
                foreach (MutableTuple<string, string, ushort> levelData in trackData)
                {
                    if (NewLevelTracks.ContainsKey(levelData.Item1))
                    {
                        levelData.Item3 = NewLevelTracks[levelData.Item1];
                    }
                    else
                    {
                        NewLevelTracks.Add(levelData.Item1, levelData.Item3);
                    }
                }

                sm.GameTrackData = trackData;

                CompareTrackData(sm.GameTrackData, NewLevelTracks);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseAllScriptManagers();
            }
        }

        private void CompareTrackData(List<MutableTuple<string, string, ushort>> trackData, Dictionary<string, ushort> expectedResults)
        {
            Dictionary<string, ushort> trackMap = new Dictionary<string, ushort>();
            foreach (MutableTuple<string, string, ushort> levelData in trackData)
            {
                trackMap.Add(levelData.Item1, levelData.Item3);
            }

            CollectionAssert.AreEquivalent(expectedResults, trackMap);
        }
    }
}