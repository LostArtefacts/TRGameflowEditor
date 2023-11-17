namespace TRGE.Coord
{
    public class TRBackupRestoreEventArgs : EventArgs
    {
        public int ProgressValue { get; internal set; }
        public int ProgressTarget { get; internal set; }

        public bool IsComplete => ProgressValue >= ProgressTarget;
    }
}