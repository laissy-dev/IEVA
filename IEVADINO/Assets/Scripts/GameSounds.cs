using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameSounds : MonoBehaviour
{
    public GameManager Game;
    public SurvivorManager Survivors;
    public Chest[] Chests;

    public AudioClip ColorTaken;
    public AudioClip SurvivorSwitched;
    public AudioClip ChestOpened;
    public AudioClip FragmentCollected;
    public AudioClip SurvivorCaught;
    public AudioClip Victory;
    public AudioClip Defeat;

    private AudioSource source;
    private PlayerColor[] colors;

    void Awake()
    {
        source = GetComponent<AudioSource>();

        colors = new PlayerColor[Survivors.Survivors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Survivors.Survivors[i].GetComponent<PlayerColor>();
        }
    }

    void OnEnable()
    {
        foreach (PlayerColor color in colors) color.OnColorChanged += HandleColorChanged;
        foreach (Survivor survivor in Survivors.Survivors) survivor.OnCaught += HandleCaught;
        foreach (Chest chest in Chests) chest.OnOpened += HandleChestOpened;

        Survivors.OnSwitchedByPlayer += HandleSwitched;
        Game.OnFragmentCollected += HandleFragmentCollected;
        Game.OnVictory += HandleVictory;
        Game.OnDefeat += HandleDefeat;
    }

    void OnDisable()
    {
        foreach (PlayerColor color in colors) color.OnColorChanged -= HandleColorChanged;
        foreach (Survivor survivor in Survivors.Survivors) survivor.OnCaught -= HandleCaught;
        foreach (Chest chest in Chests) chest.OnOpened -= HandleChestOpened;

        Survivors.OnSwitchedByPlayer -= HandleSwitched;
        Game.OnFragmentCollected -= HandleFragmentCollected;
        Game.OnVictory -= HandleVictory;
        Game.OnDefeat -= HandleDefeat;
    }

    void HandleColorChanged(ColorData color)
    {
        if (color != null) Play(ColorTaken);
    }

    void HandleSwitched(Survivor survivor) => Play(SurvivorSwitched);
    void HandleCaught(Survivor survivor) => Play(SurvivorCaught);
    void HandleChestOpened(Chest chest) => Play(ChestOpened);
    void HandleFragmentCollected(int count) => Play(FragmentCollected);
    void HandleVictory() => Play(Victory);
    void HandleDefeat() => Play(Defeat);

    void Play(AudioClip clip)
    {
        if (clip != null) source.PlayOneShot(clip);
    }
}
