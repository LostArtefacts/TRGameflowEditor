using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    public class TRFileEventArgs : EventArgs
    {
        public string FilePath { get; private set; }

        public TRFileEventArgs(string path)
        {
            FilePath = path;
        }
    }
}