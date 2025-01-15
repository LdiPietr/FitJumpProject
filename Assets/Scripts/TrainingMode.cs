using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingMode : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    void Start()
    {
        Time.timeScale = 1;
    }

    public void RestartTraining()
    {
        GameManager.Instance.StartGame(false);
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.EndGame();
    }
}