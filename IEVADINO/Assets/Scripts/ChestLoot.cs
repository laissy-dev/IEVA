using System.Collections.Generic;
using UnityEngine;

public class ChestLoot : MonoBehaviour
{
    public Chest[] Chests;
    public AntidoteFragment[] Fragments;

    void Awake()
    {
        List<Chest> available = new List<Chest>(Chests);

        foreach (AntidoteFragment fragment in Fragments)
        {
            fragment.gameObject.SetActive(false);
            if (available.Count == 0)
            {
                Debug.LogWarning("[ChestLoot] Pas assez de coffres pour tous les fragments.");
                continue;
            }

            int index = Random.Range(0, available.Count);
            available[index].Content = fragment;
            available.RemoveAt(index);
        }
    }
}
