using System.Collections.Generic;

namespace TRGE.Core
{
    public abstract class AbstractTRGEEditor : ITRSaveProgressProvider
    {
        protected Dictionary<string, object> _config;

        internal bool AllowSuccessiveEdits { get; set; }
        internal virtual bool IsExportPossible => _config != null;

        public abstract int GetSaveTargetCount();

        internal virtual Dictionary<string, object> ExportConfig()
        {
            return new Dictionary<string, object>(_config);
        }

        internal void ImportConfig(Dictionary<string, object> config)
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

        protected abstract void ReadConfig(Dictionary<string, object> config);
        protected abstract void ApplyConfig(Dictionary<string, object> config);

        internal abstract void Restore();
    }
}