using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;

    // Fonction apelé à chaque activation du monobehaviour
    private void OnEnable()
    {   
        // Bind entre la fonction UpdateScore et l'action OnTargetCollected
        Player_Collect.OnTargetCollected += UpdateScore;
    }

    // Fonction désactivé à chaque désactivation du monobehaviour
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
