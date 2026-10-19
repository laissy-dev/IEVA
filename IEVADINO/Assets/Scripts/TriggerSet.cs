using System.Collections.Generic;

// Un objet composé de plusieurs colliders déclenche un OnTriggerEnter par collider :
// on compte les entrées pour ne le considérer sorti qu'au dernier OnTriggerExit.
public class TriggerSet<T> where T : class
{
    private readonly Dictionary<T, int> counts = new Dictionary<T, int>();

    public IEnumerable<T> Items => counts.Keys;

    // Renvoie true si l'objet vient d'entrer.
    public bool Add(T item)
    {
        counts.TryGetValue(item, out int count);
        counts[item] = count + 1;
        return count == 0;
    }

    // Renvoie true si l'objet vient de sortir.
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
