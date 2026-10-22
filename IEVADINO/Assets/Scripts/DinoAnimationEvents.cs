using UnityEngine;

// Reçoit les Animation Events des clips du Chomper (pas, début et fin d'attaque).
[RequireComponent(typeof(AudioSource))]
public class DinoAnimationEvents : MonoBehaviour
{
    public AudioClip[] Steps;
    public AudioClip Roar;

    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayStep()
    {
        if (Steps.Length == 0) return;

        source.pitch = Random.Range(0.9f, 1.1f);
        source.PlayOneShot(Steps[Random.Range(0, Steps.Length)]);
    }

    public void AttackBegin()
    {
        if (Roar != null) source.PlayOneShot(Roar);
    }

    public void AttackEnd()
    {
    }
}
