namespace TRGE.View.Model.Data;

public class BaseLevelData
{
    public string LevelID { get; private set; }
    public string LevelName { get; private set; }

    public BaseLevelData(string levelID, string levelName)
    {
        LevelID = levelID;
        LevelName = levelName;
    }
}