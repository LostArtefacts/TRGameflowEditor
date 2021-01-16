﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TRGE.Core.Test
{
    class Program
    {
        private static int _pass, _fail;

        static void Main()
        {
            _pass = _fail = 0;

            #region BasicIO
            RunTest(new TR2PCScriptIOTests());
            RunTest(new TR2GPCScriptIOTests());

            RunTest(new TR2PSXScriptIOTests());
            RunTest(new TR2PSXBetaScriptIOTests());

            RunTest(new TR3PCScriptIOTests());
            RunTest(new TR3GPCScriptIOTests());

            RunTest(new TR3PSXScriptIOTests());
            #endregion

            #region Flags
            RunTest(new TR23FlagTests());
            RunTest(new TR23MiscTests());
            #endregion

            #region ScriptManagement
            RunTest(new TR23ScriptManagementTests());
            #endregion

            #region Levels
            RunTest(new TR2PCLevelTests());
            RunTest(new TR2GPCLevelTests());

            RunTest(new TR2PSXLevelTests());
            RunTest(new TR2PSXBetaLevelTests());

            RunTest(new TR3PCLevelTests());
            RunTest(new TR3GPCLevelTests());
            RunTest(new TR3PSXLevelTests());
            #endregion

            #region Unarmed
            RunTest(new TR2PCUnarmedTests());
            RunTest(new TR2GPCUnarmedTests());

            RunTest(new TR2PSXUnarmedTests());
            RunTest(new TR2PSXBetaUnarmedTests());

            RunTest(new TR3PCUnarmedTests());
            RunTest(new TR3GPCUnarmedTests());
            RunTest(new TR3PSXUnarmedTests());
            #endregion

            #region Items
            RunTest(new TR2PCItemTests());
            RunTest(new TR2PCGItemTests());

            RunTest(new TR2PSXItemTests());
            RunTest(new TR2PSXBetaItemTests());

            RunTest(new TR3PCItemTests());
            RunTest(new TR3GPCItemTests());
            RunTest(new TR3PSXItemTests());
            #endregion

            #region Interruptions
            RunTest(new TR2PCInterruptionTests());
            #endregion

            WriteHeader("Test Results");
            Console.WriteLine("PASS: {0}", _pass);
            Console.WriteLine("FAIL: {0}", _fail);
            Console.Read();
        }

        private static void RunTest(AbstractTestCollection testCollection)
        {
            WriteHeader(testCollection.GetType().ToString());
            Dictionary<string, Exception> results = testCollection.Run();
            foreach (string methodName in results.Keys)
            {
                Exception e = results[methodName];
                if (e == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("{0}: PASS", methodName);
                    _pass++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("{0}: FAIL", methodName);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(e);
                    _fail++;
                }
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        private static void WriteHeader(string header)
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