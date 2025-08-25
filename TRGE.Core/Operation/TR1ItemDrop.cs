namespace TRGE.Core;

public class TR1ItemDrop
{
    public int EnemyNum { get; set; }
    public List<int> ObjectIds { get; set; }

    public void Replace<T>(int index, T type)
        where T : Enum
    {
        if (index >= 0 && index < ObjectIds.Count)
        {
            ObjectIds[index] = Convert.ToInt32(type);
        }
    }

    public void RemoveAll<T>(IEnumerable<T> ids)
        where T : Enum
    {
        foreach (var id in ids.Select(i => Convert.ToInt32(i)))
        {
            ObjectIds?.Remove(id);
        }
    }
}
