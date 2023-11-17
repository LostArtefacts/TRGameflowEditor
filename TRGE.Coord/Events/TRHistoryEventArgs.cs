namespace TRGE.Coord
{
    public class TRHistoryEventArgs : EventArgs
    {
        public string Path { get; private set; }

        public TRHistoryEventArgs(string path)
        {
            Path = path;
        }
    }
}