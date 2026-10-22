using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DinoAnimationEvents : MonoBehaviour
{
    public AudioClip[] Steps;
    public AudioClip Roar;
    public bool OnlyWhenChasing = true;

    private AudioSource source;
    private DinoController dino;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        dino = GetComponentInParent<DinoController>();
    }

    public void PlayStep()
    {
        if (Steps.Length == 0) return;
        if (OnlyWhenChasing && dino != null && !dino.TargetVisible) return;

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
