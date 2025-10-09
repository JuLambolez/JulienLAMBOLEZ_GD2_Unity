using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;

    // Fonction apel� � chaque activation du monobehaviour
    private void OnEnable()
    {   
        // Bind entre la fonction UpdateScore et l'action OnTargetCollected
        Player_Collect.OnTargetCollected += UpdateScore;
    }

    // Fonction d�sactiv� � chaque d�sactivation du monobehaviour
    private void OnDisable()
    {
        // Unbind entre la fonction UpdateScore et l'action OnTargetCollected  
        Player_Collect.OnTargetCollected -= UpdateScore;
    }

    private void Start()
    {
        UpdateScore(0);
    }

    public void UpdateScore(int newScore)
    {
        _scoreText.text = $"Score : {newScore.ToString()}";
    }
}
