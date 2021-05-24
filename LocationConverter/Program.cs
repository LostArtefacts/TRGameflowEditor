using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TRViewInterop.Routes;

namespace LocationConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && File.Exists(args[0]))
            {
                string file = args[0];
                string fileName = Path.GetFileNameWithoutExtension(file);
                List<TRViewLocation> routeLocations = RouteToLocationsConverter.Convert(file);

                if (File.Exists("default_locations.json"))
                {
                    // Insert the old default location as the first one, so we default to that if randomization not used
                    Dictionary<string, TRViewLocation> defaultLocations = JsonConvert.DeserializeObject<Dictionary<string, TRViewLocation>>(File.ReadAllText("default_locations.json"));
                    string key = fileName.ToUpper() + ".TR2";
                    if (defaultLocations.ContainsKey(key))
                    {
                        routeLocations.Insert(0, defaultLocations[key]);
                    }
                }

                File.WriteAllText(Path.Combine(Path.GetDirectoryName(file), fileName + ".json"), JsonConvert.SerializeObject(routeLocations, Formatting.Indented));
            }
            else
            {
                List<TRViewLocation> locations = JsonConvert.DeserializeObject<List<TRViewLocation>>(File.ReadAllText("xian.rando.txt"));
                for (int i = locations.Count - 1; i >= 0; i--)
                {
                    TRViewLocation loc = locations[i];
                    if (loc.Room != 0 && loc.Room != 3 && loc.Room != 17)
                    {
                        locations.RemoveAt(i);
                    }
                }
                File.WriteAllText("xian.json", JsonConvert.SerializeObject(locations, Formatting.Indented));
            }
        }
    }
}