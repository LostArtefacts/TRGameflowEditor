using System;
using TRGE.Coord;

namespace TRGE.View.Model;

public class DataFolderEventArgs : EventArgs
{
    public string DataFolder { get; private set; }
    public TREditor Editor { get; private set; }

    public DataFolderEventArgs(string dataFolder, TREditor editor)
    {
        DataFolder = dataFolder;
        Editor = editor;
    }
}