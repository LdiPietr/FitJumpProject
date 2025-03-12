using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    private static InGameUIManager _instance;

    public static InGameUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindFirstObjectByType<InGameUIManager>();
            }

            return _instance;
        }
    }

    [Header("UI Elements")] public TextMeshProUGUI scoreText;
    public string comboText;
    public TextMeshProUGUI highScoreText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI bestComboText;

    [Header("Power-up UI")] public Image shieldTimer;
    public Image jetpackTimer;
    public GameObject powerUpPanel;

    private void Awake()
    {
        _instance = this;
        Debug.Log("InGameUIManager initialized");
    }

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        UpdateScoreText();
        UpdateComboText();
        gameOverPanel.SetActive(false);
        //powerUpPanel.SetActive(false);
        // shieldTimer.fillAmount = 0;
        // jetpackTimer.fillAmount = 0;
    }

    public void UpdateScoreText()
    {
        UpdateComboText();
        scoreText.text = $"{Mathf.FloorToInt(GameplayManager.Instance.score):D8}" + comboText;
    }

    public void UpdateComboText()
    {
        comboText = $" (x{GameplayManager.Instance.comboMultiplier:0.0})";
        scoreText.text = $" {Mathf.FloorToInt(GameplayManager.Instance.score):D8}" + comboText;
    }

    public void UpdatePowerUpTimer(string powerUpType, float remainingTime, float totalTime)
    {
        switch (powerUpType)
        {
            case "shield":
                shieldTimer.fillAmount = remainingTime / totalTime;
                break;
            case "jetpack":
                jetpackTimer.fillAmount = remainingTime / totalTime;
                break;
        }
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = $"Final Score: {Mathf.FloorToInt(GameplayManager.Instance.score)}";
        SaveHighScore();
    }

    private void SaveHighScore()
    {
        int currentScore = Mathf.FloorToInt(GameplayManager.Instance.score);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }

        highScoreText.text = $"High Score: {PlayerPrefs.GetInt("HighScore")}";
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}