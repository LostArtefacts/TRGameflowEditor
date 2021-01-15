using System;
using System.Collections.Generic;

namespace TRGE.Core
{
    public class TR23ScriptManager : AbstractTRScriptManager
    {
        internal TR23ScriptManager(string originalFilePath)
            : base(originalFilePath, new TR23Script()) { }

        internal TR23ScriptManager(string originalFilePath, TR23Script script)
            : base(originalFilePath, script) { }

        protected override Dictionary<string, object> CreateConfig()
        {
            throw new NotImplementedException();
        }

        protected override void LoadConfig(Dictionary<string, object> config)
        {
            throw new NotImplementedException();
        }

        protected override void PrepareSave()
        {
            throw new NotImplementedException();
        }

        internal override AbstractTRScript LoadBackupScript()
        {
            TR23Script backupScript = new TR23Script();
            backupScript.Read(BackupFilePath);
            return backupScript;
        }

        public List<MutableTuple<string, string, bool>> UnarmedLevelData
        {
            get => (LevelManager as TR23LevelManager).GetUnarmedLevelData();
            set => (LevelManager as TR23LevelManager).SetUnarmedLevelData(value);
        }

        public Organisation UnarmedLevelOrganisation
        {
            get => (LevelManager as TR23LevelManager).UnarmedLevelOrganisation;
            set => (LevelManager as TR23LevelManager).UnarmedLevelOrganisation = value;
        }

        public RandomGenerator UnarmedLevelRNG
        {
            get => (LevelManager as TR23LevelManager).UnarmedLevelRNG;
            set => (LevelManager as TR23LevelManager).UnarmedLevelRNG = value;
        }

        public uint RandomUnarmedLevelCount
        {
            get => (LevelManager as TR23LevelManager).RandomUnarmedLevelCount;
            set => (LevelManager as TR23LevelManager).RandomUnarmedLevelCount = value;
        }

        internal void RandomiseUnarmedLevels()
        {
            (LevelManager as TR23LevelManager).RandomiseUnarmedLevels(LoadBackupScript().Levels);
        }

        internal List<TR23Level> GetUnarmedLevels()
        {
            return (LevelManager as TR23LevelManager).GetUnarmedLevels();
        }
    }
}