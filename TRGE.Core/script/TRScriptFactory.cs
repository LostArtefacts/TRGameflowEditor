using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    internal static class TRScriptFactory
    {
        internal static AbstractTRScriptManager GetScriptManager(string filePath)
        {
            switch (GetDatFileVersion(filePath))
            {
                case TR23Script.Version:
                    return new TR23ScriptManager(filePath);
                default:
                    throw new UnsupportedScriptException();
            }
        }

        internal static AbstractTRScript OpenScript(string filePath)
        {
            AbstractTRScript script;
            switch (GetDatFileVersion(filePath))
            {
                case TR23Script.Version:
                    script = new TR23Script();
                    break;
                default:
                    throw new UnsupportedScriptException();
            }

            script.Read(filePath);
            return script;
        }

        private static uint GetDatFileVersion(string filePath)
        {
            using (BinaryReader br = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                return br.ReadUInt32();
            }
        }
    }
}