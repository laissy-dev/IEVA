using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Antidote : MonoBehaviour
{
    public event Action OnCollected;

    private bool collected;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (other.attachedRigidbody == null || !other.attachedRigidbody.CompareTag("Player")) return;

        collected = true;
        OnCollected?.Invoke();
        gameObject.SetActive(false);
    }
}
