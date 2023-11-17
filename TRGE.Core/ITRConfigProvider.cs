namespace TRGE.Core;

public interface ITRConfigProvider
{
    object GetConfig();
    void SetConfig(object config);
}