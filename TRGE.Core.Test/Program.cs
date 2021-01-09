using System;
using System.Collections.Generic;
using System.Text;

namespace TRGE.Core.Test
{
    class Program
    {
        static void Main()
        {
            RunTest(new TR2PCScriptReadTests());
            RunTest(new TR2PSXScriptReadTests());
            //RunTest(new TR2ScriptWriteTests());
            Console.Read();
        }

        static void RunTest(AbstractTestCollection testCollection)
        {
            WriteHeader(testCollection.GetType().ToString());
            Dictionary<string, Exception> results = testCollection.Run();
            foreach (string methodName in results.Keys)
            {
                Exception e = results[methodName];
                Console.ForegroundColor = e == null ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(methodName + ": " + (e == null ? "PASS" : "FAIL"));
                if (e != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(e);
                }
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        static void WriteHeader(string header)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < header.Length + 4; i++)
            {
                sb.Append("*");
            }

            Console.WriteLine(sb);
            Console.WriteLine("* {0} *", header);
            Console.WriteLine(sb);
        }
    }
}