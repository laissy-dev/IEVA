using System;
using System.Collections;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public GameRules Rules;

    public int SecondsLeft { get; private set; }

    public event Action<int> OnTick;
    public event Action OnTimeUp;

    IEnumerator Start()
    {
        SecondsLeft = Rules.TimeLimit;
        OnTick?.Invoke(SecondsLeft);

        WaitForSeconds second = new WaitForSeconds(1.0f);
        while (SecondsLeft > 0)
        {
            yield return second;
            SecondsLeft--;
            OnTick?.Invoke(SecondsLeft);
        }
        OnTimeUp?.Invoke();
    }
}
