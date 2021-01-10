using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    internal class AbstractTRScriptManager
    {
        public AbstractTRLevelManager LevelManager { get; private set; }
        internal AbstractTRScript Script { get; private set; }
        public TREdition Edition => Script.Edition;
        public string OriginalFilePath { get; internal set; }
        public string BackupFilePath { get; internal set; }

        internal AbstractTRScriptManager(string originalFilePath, AbstractTRScript script)
        {
            Initialise(originalFilePath, script);
        }

        private void Initialise(string originalFilePath, AbstractTRScript script)
        {
            OriginalFilePath = originalFilePath;
            //CreateBackup();

            Script = script;
            Script.Read(OriginalFilePath);

            LevelManager = TRLevelFactory.GetLevelManager(script);

            //ReadConfig();
        }
    }
}