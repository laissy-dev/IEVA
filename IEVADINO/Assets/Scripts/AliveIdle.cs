using UnityEngine;

public class AliveIdle : MonoBehaviour
{
    public enum Mode { Flotter, Respirer, Balancer, Tourner, Vent }

    public Mode Style = Mode.Respirer;
    public float Amplitude = 1.0f;
    public float Speed = 1.0f;

    private Vector3 basePosition;
    private Quaternion baseRotation;
    private Vector3 baseScale;
    private float phase;

    void Start()
    {
        basePosition = transform.localPosition;
        baseRotation = transform.localRotation;
        baseScale = transform.localScale;
        phase = Random.value * 10.0f;
    }

    void Update()
    {
        float time = Time.time * Speed + phase;
        float wave = Mathf.Sin(time * 2.0f);

        switch (Style)
        {
            case Mode.Flotter:
                transform.localPosition = basePosition + Vector3.up * wave * 0.1f * Amplitude;
                break;
            case Mode.Respirer:
                float stretch = wave * 0.04f * Amplitude;
                transform.localScale = new Vector3(baseScale.x * (1.0f - stretch * 0.5f), baseScale.y * (1.0f + stretch), baseScale.z * (1.0f - stretch * 0.5f));
                break;
            case Mode.Balancer:
                transform.localRotation = baseRotation * Quaternion.Euler(0.0f, 0.0f, wave * 6.0f * Amplitude);
                break;
            case Mode.Tourner:
                transform.localRotation = baseRotation * Quaternion.Euler(0.0f, time * 60.0f * Amplitude, 0.0f);
                break;
            case Mode.Vent:
                // Rafales irrégulières décalées selon la position : la vague de vent traverse la forêt.
                Vector3 p = transform.position;
                float gust = Mathf.PerlinNoise(Time.time * Speed * 0.3f + p.x * 0.05f, p.z * 0.05f);
                transform.localRotation = baseRotation * Quaternion.Euler(gust * 8.0f * Amplitude, 0.0f, wave * 1.5f * Amplitude);
                break;
        }
    }
}
