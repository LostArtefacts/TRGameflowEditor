namespace TRGE.Core
{
    public abstract class AbstractTRGEEditor : ITRSaveProgressProvider
    {
        protected Config _config;
        internal abstract string ConfigFilePath { get; }

        internal virtual bool IsExportPossible => _config != null;

        public abstract int GetSaveTargetCount();

        internal virtual Config ExportConfig()
        {
            return new Config(_config);
        }

        internal void ImportConfig(Config config)
        {
            try
            {
                ReadConfig(config);
                _config = config;
            }
            catch
            {
                ReadConfig(_config);
                throw;
            }
        }

        protected abstract void ReadConfig(Config config);
        protected abstract void ApplyConfig(Config config);

        internal abstract void Restore();
        internal abstract void SaveComplete();
    }
}