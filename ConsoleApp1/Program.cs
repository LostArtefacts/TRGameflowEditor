using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TRViewInterop.Routes;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            List<TRViewLocation> routeLocations = RouteToLocationsConverter.Convert(@"C:\users\lewis\Desktop\opera.tvr");
            List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();
            Dictionary<string, List<Dictionary<string, object>>> map = new Dictionary<string, List<Dictionary<string, object>>>
            {
                ["OPERA.TR2"] = lst
            };
            foreach (TRViewLocation loc in routeLocations)
            {
                lst.Add(new Dictionary<string, object>
                { 
                    ["X"] = loc.X,
                    ["Z"] = loc.Z,
                    ["Y"] = loc.Y,
                    ["Room"] = loc.Room
                });
            }
            File.WriteAllText(@"C:\users\lewis\Desktop\ualocations.json", JsonConvert.SerializeObject(map, Formatting.Indented));
            Console.WriteLine();
        }
    }
}
