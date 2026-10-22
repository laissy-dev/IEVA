using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public GameManager Game;
    public GameObject Panel;
    public Text Title;

    public string VictoryText = "Victoire ! L'antidote est reconstitué";
    public string DefeatText = "Extinction... Les survivants ont été dévorés";
    public string TimeOutText = "Trop tard... L'antidote n'a pas été reconstitué à temps";

    void Awake()
    {
        Panel.SetActive(false);
    }

    void OnEnable()
    {
        Game.OnVictory += ShowVictory;
        Game.OnDefeat += ShowDefeat;
    }

    void OnDisable()
    {
        Game.OnVictory -= ShowVictory;
        Game.OnDefeat -= ShowDefeat;
    }

    void ShowVictory() => Show(VictoryText);
    void ShowDefeat() => Show(Game.TimedOut ? TimeOutText : DefeatText);

    void Show(string text)
    {
        Title.text = text;
        Panel.SetActive(true);
    }

    public void Replay()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}
