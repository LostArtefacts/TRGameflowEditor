using System;

namespace TRGE.Core
{
    public class TRSaveMonitor
    {
        public event EventHandler<TRSaveEventArgs> SaveStateChanged;

        private readonly TRSaveEventArgs _args;

        public TRSaveMonitor(TRSaveEventArgs e)
        {
            _args = e;
        }

        public void FireSaveStateChanged(int progress = 0, TRSaveCategory category = TRSaveCategory.None)
        {
            _args.ProgressValue += progress;
            _args.Category = category;
            SaveStateChanged?.Invoke(this, _args);
        }
    }
}