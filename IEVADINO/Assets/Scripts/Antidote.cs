using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Antidote : MonoBehaviour
{
    private bool victoire = false;

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;

        Rigidbody body = gameObject.AddComponent<Rigidbody>();
        body.isKinematic = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            victoire = true;
            GetComponent<Renderer>().enabled = false;
            Time.timeScale = 0;
        }
    }

    void OnGUI()
    {
        if (!victoire) return;
        GUI.Label(new Rect(20, 20, 500, 30), "Victoire ! Antidote récupéré");
    }
}
