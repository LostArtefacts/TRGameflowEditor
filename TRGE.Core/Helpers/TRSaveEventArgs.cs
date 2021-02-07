using System;

namespace TRGE.Core
{
    public class TRSaveEventArgs : EventArgs
    {
        public int ProgressValue { get; internal set; }
        public int ProgressTarget { get; internal set; }
        public string ProgressDescription { get; internal set; }
    }
}