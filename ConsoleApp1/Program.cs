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
            using (BinaryReader br = new BinaryReader(new FileStream(@"C:\Users\Lewis\Desktop\hsh\try2\Pistol.trmvb", FileMode.Open)))
            {
                byte version = br.ReadByte();
                byte trType = br.ReadByte();
                byte trVersion = br.ReadByte();
                byte padding = br.ReadByte(); //0
                uint numAnims = br.ReadUInt32();

                //uint numAnims = br.ReadUInt32();
                Console.WriteLine(version);
                Console.WriteLine(trType);
                Console.WriteLine(trVersion);
                Console.WriteLine(padding);
                Console.WriteLine(numAnims);
            }

            Console.ReadLine();
        }
    }
}
