using System.Collections.Generic;

public class TriggerSet<T> where T : class
{
    private readonly Dictionary<T, int> counts = new Dictionary<T, int>();

    public IEnumerable<T> Items => counts.Keys;

    public bool Add(T item)
    {
        counts.TryGetValue(item, out int count);
        counts[item] = count + 1;
        return count == 0;
    }

    public bool Remove(T item)
    {
        if (!counts.TryGetValue(item, out int count)) return false;
        if (count > 1)
        {
            counts[item] = count - 1;
            return false;
        }
        counts.Remove(item);
        return true;
    }
}
