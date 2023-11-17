namespace TRGE.Core
{
    public class TRDownloadEventArgs : EventArgs
    {
        public string URL { get; internal set; }
        public string TargetFile { get; internal set; }
        public long DownloadLength { get; internal set; }
        public long DownloadProgress { get; internal set; }
        public int DownloadDifference { get; internal set; }
        public Exception Exception { get; internal set; }
        public TRDownloadStatus Status { get; internal set; }
        public bool IsCancelled
        {
            get => Status == TRDownloadStatus.Cancelled;
            set => Status = value ? TRDownloadStatus.Cancelled : TRDownloadStatus.Undefined;
        }

        internal TRDownloadEventArgs()
        {
            DownloadLength = DownloadProgress = 0;
            Status = TRDownloadStatus.Initialising;
        }
    }

    public enum TRDownloadStatus
    {
        Initialising, Downloading, Committing, Completed, Failed, Cancelled, Undefined
    }
}