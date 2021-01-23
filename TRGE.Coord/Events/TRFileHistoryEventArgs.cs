using System;
using System.IO;

namespace TRGE.Coord
{
    public class TRFileHistoryEventArgs : EventArgs
    {
        public FileInfo File { get; private set; }

        public TRFileHistoryEventArgs(FileInfo file)
        {
            File = file;
        }
    }
}