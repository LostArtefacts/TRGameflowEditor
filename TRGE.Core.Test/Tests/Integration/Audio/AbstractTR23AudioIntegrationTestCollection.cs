using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    public abstract class AbstractTR23AudioIntegrationTestCollection : BaseTestCollection
    {
        protected abstract int ScriptFileIndex { get; }

        //[TestMethod]
        protected void TestAudioMapping()
        {
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;
            
            List<MutableTuple<string, string, ushort>> trackData = sm.GameTrackData;
            Console.WriteLine("Audio mapping tests - start the game after each prompt and verify the title screen matches.");
            Console.WriteLine("Press S to skip individual tracks or Q to skip this test altogether.");
            Console.Write("Enter the full path to the dat file for the game (e.g. Steam folder). Make sure to backup any existing file. Enter Q to skip: ");

            string outputPath = Console.ReadLine();
            if (!outputPath.ToLower().Equals("q"))
            {
                sm.FrontEndHasFMV = false;
                for (int i = 0; i < sm.LevelManager.AudioProvider.Tracks.Count; i++)
                {
                    TRAudioTrack track = sm.LevelManager.AudioProvider.Tracks[i];
                    trackData[0].Item3 = track.ID;
                    sm.GameTrackData = trackData;
                    editor.Save();
                    File.Copy(_validScripts[ScriptFileIndex], outputPath, true);

                    Console.WriteLine("Test {0} of {1}", i + 1, sm.LevelManager.AudioProvider.Tracks.Count);
                    Console.WriteLine("Script saved to {0}", outputPath);
                    Console.WriteLine("Start game and verify title screen sound is {0}", track.ToString());
                    Console.Write("Match? [Y]es [N]o [S]kip track [Q]uit test: ");
                    string s = Console.ReadLine().ToLower();
                    if (s.Equals("s"))
                    {
                        Console.WriteLine();
                        continue;
                    }
                    if (s.Equals("q"))
                    {
                        Console.WriteLine();
                        break;
                    }
                    if (!s.ToLower().Equals("y"))
                    {
                        Assert.Fail();
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}