using UnityEngine;
using UnityEngine.UI;

public class TimerBoard : MonoBehaviour
{
    public GameTimer Timer;
    public Text Label;
    public int WarningSeconds = 60;
    public Color WarningColor = Color.red;

    private Color normalColor;

    void Awake()
    {
        normalColor = Label.color;
    }

    void OnEnable()
    {
        Timer.OnTick += Show;
    }

    void OnDisable()
    {
        Timer.OnTick -= Show;
    }

    void Show(int secondsLeft)
    {
        Label.text = (secondsLeft / 60) + ":" + (secondsLeft % 60).ToString("00");
        Label.color = secondsLeft <= WarningSeconds ? WarningColor : normalColor;
    }
}
