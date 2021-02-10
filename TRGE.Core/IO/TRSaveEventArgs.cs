using System;

namespace TRGE.Core
{
    public class TRSaveEventArgs : EventArgs
    {
        public int ProgressValue { get; internal set; }
        public int ProgressTarget { get; internal set; }
        public TRSaveCategory Category { get; internal set; }

        public TRSaveEventArgs()
        {
            ProgressValue = 0;
            ProgressTarget = 0;
            Category = TRSaveCategory.None;
        }
    }
}