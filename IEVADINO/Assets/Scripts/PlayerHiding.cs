using UnityEngine;

[RequireComponent(typeof(PlayerColor))]
public class PlayerHiding : MonoBehaviour
{
    public float HideDuration = 20.0f;
    public float ColorTolerance = 0.1f;

    public bool IsHidden;
    public float TimeLeft;
    public string ZoneUnderFeet = "";

    private PlayerColor playerColor;
    private Color lastColor;

    void Start()
    {
        playerColor = GetComponent<PlayerColor>();
        lastColor = playerColor.CurrentColor;
    }

    void Update()
    {
        if (playerColor.CurrentColor != lastColor)
        {
            lastColor = playerColor.CurrentColor;
            TimeLeft = playerColor.CurrentColorName == "" ? 0.0f : HideDuration;
        }

        ColorSource zone = FindZoneUnderFeet();
        ZoneUnderFeet = zone != null ? zone.ColorName : "";

        IsHidden = zone != null && TimeLeft > 0.0f && SameColor(zone.GetColor(), playerColor.CurrentColor);

        if (IsHidden)
        {
            TimeLeft -= Time.deltaTime;
            if (TimeLeft <= 0.0f)
            {
                TimeLeft = 0.0f;
                IsHidden = false;
                playerColor.ApplyColor(Color.white, "");
            }
        }
    }

    ColorSource FindZoneUnderFeet()
    {
        Vector3 origin = transform.position + Vector3.up;
        Debug.DrawRay(origin, Vector3.down * 3.0f, Color.magenta);

        foreach (RaycastHit hit in Physics.RaycastAll(origin, Vector3.down, 3.0f, ~0, QueryTriggerInteraction.Ignore))
        {
            ColorSource source = hit.collider.GetComponent<ColorSource>();
            if (source != null) return source;
        }
        return null;
    }

    bool SameColor(Color a, Color b)
    {
        Vector3 rgbA = new Vector3(a.r, a.g, a.b);
        Vector3 rgbB = new Vector3(b.r, b.g, b.b);
        return Vector3.Distance(rgbA, rgbB) < ColorTolerance;
    }

    void OnGUI()
    {
        string state = IsHidden ? "Caché (" + Mathf.CeilToInt(TimeLeft) + " s)" : "Visible";
        GUI.Label(new Rect(20, 50, 500, 30), state);
    }
}
