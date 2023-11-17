namespace TRGE.Core;

public class TRSaveMonitor
{
    public event EventHandler<TRSaveEventArgs> SaveStateChanged;

    private readonly TRSaveEventArgs _args;

    public bool IsCancelled
    {
        get => _args.IsCancelled;
        set => _args.IsCancelled = value;
    }

    public TRSaveMonitor(TRSaveEventArgs e)
    {
        _args = e;
    }

    public void FireSaveStateBeginning(TRSaveCategory category = TRSaveCategory.None, string customDescription = null)
    {
        FireSaveStateChanged(0, category, customDescription);
    }

    public void FireSaveStateChanged(int progress = 0, TRSaveCategory category = TRSaveCategory.None, string customDescription = null)
    {
        _args.ProgressValue += progress;
        _args.Category = category;
        _args.CustomDescription = customDescription;
        SaveStateChanged?.Invoke(this, _args);
    }
}