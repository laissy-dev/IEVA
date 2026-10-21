using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AntidoteFragment : MonoBehaviour
{
    public Collider PickupTrigger;
    public float PickupDelay = 0.6f;
    public float EjectSpeed = 4.0f;
    public float EjectSpread = 0.25f;

    public event Action<AntidoteFragment> OnCollected;

    private bool collected;

    public void Reveal(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);

        Vector2 side = UnityEngine.Random.insideUnitCircle * EjectSpread;
        GetComponent<Rigidbody>().AddForce(new Vector3(side.x, 1.0f, side.y) * EjectSpeed, ForceMode.VelocityChange);

        StartCoroutine(EnablePickup());
    }

    IEnumerator EnablePickup()
    {
        PickupTrigger.enabled = false;
        yield return new WaitForSeconds(PickupDelay);
        PickupTrigger.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (other.attachedRigidbody == null || !other.attachedRigidbody.CompareTag("Player")) return;

        collected = true;
        OnCollected?.Invoke(this);
        gameObject.SetActive(false);
    }
}
