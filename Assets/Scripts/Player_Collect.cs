using System;
using UnityEngine;

public class Player_Collect : MonoBehaviour
{
    [SerializeField] private ScoreDatas _scoreData;

    public static Action<int> OnTargetCollected;

    public void UpdateScore(int value)
    {
        _scoreData.ScoreValue += value;
        _scoreData.ScoreValue = Mathf.Max(_scoreData.ScoreValue, -100);

        OnTargetCollected?.Invoke(_scoreData.ScoreValue);
    }
}
