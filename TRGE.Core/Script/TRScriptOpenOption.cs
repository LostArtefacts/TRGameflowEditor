namespace TRGE.Core
{
    public enum TRScriptOpenOption
    {
        Default, 
        DiscardBackup, //whichever file is opened will replace any current backup
        RestoreBackup  //restore the backup first then open
    }
}