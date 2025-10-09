using System;
using UnityEngine;

public class Player_Collect : MonoBehaviour
{
    [SerializeField] private ScoreDatas _scoreData;
    [SerializeField] private UIController _uiController;
    //Définition de l'action (event dispatcher), avec l'input entre <> ici un int
    public static Action<int> OnTargetCollected;

    public void UpdateScore(int value)
    {
        _scoreData.ScoreValue = Mathf.Clamp(_scoreData.ScoreValue + value, min:-100 ,max: _scoreData.ScoreValue + value);
        //_uiController.UpdateScore(_scoreData.ScoreValue);
        // Call event dispatcher, en C# on invoque avec l'input entre parenthèses
        OnTargetCollected?.Invoke(_scoreData.ScoreValue);
    }
}
