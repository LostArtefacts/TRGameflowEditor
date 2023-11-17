namespace TRGE.Core;

internal class TRItemBrokerDealer<T> : List<T> where T : BaseTRItemBroker
{
    internal T Get(Random rand)
    {
        int r = rand.Next(0, GetTotalWeight());
        foreach (T broker in this)
        {
            if (r <= broker.Weight)
            {
                return broker;
            }
            r -= broker.Weight;
        }
        return null;
    }

    internal int GetTotalWeight()
    {
        int total = 0;
        foreach (T broker in this)
        {
            total += broker.Weight;
        }
        return total;
    }
}