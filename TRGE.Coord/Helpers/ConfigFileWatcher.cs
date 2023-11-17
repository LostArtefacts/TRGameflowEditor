namespace TRGE.Coord;

internal class ConfigFileWatcher
{
    private static readonly DateTime _never = new(1970, 1, 1);

    private readonly FileSystemWatcher _watcher;
    private readonly string _filePath;
    private DateTime _lastModified;

    public bool Enabled
    {
        get => _watcher.EnableRaisingEvents;
        set
        {
            if (value)
            {
                ResetLastModified();
            }
            _watcher.EnableRaisingEvents = value;
        }
    }

    public event EventHandler<FileSystemEventArgs> Changed;

    internal ConfigFileWatcher(string filePath)
    {
        FileInfo fi = new(_filePath = filePath);
        _lastModified = File.Exists(_filePath) ? fi.LastWriteTime : _never;

        _watcher = new FileSystemWatcher
        {
            Path = fi.DirectoryName,
            Filter = fi.Name,
            EnableRaisingEvents = true
        };

        _watcher.Changed += Watcher_Changed;
    }

    private void ResetLastModified()
    {
        if (File.Exists(_filePath))
        {
            _lastModified = new FileInfo(_filePath).LastWriteTime;
        }
    }

    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        FileInfo fi = new(e.FullPath);
        DateTime lastModified = fi.LastWriteTime;
        if (!lastModified.Equals(_lastModified))
        {
            _lastModified = lastModified;
            Changed?.Invoke(this, e);
        }
    }
}