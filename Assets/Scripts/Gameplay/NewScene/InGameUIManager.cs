using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    private static InGameUIManager _instance;

    [Header("Countdown")] public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;

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
    
    [Header("Tournament Canvas")]
    public TextMeshProUGUI weeklyScoreText;
    public TextMeshProUGUI monthlyScoreText;
    public TextMeshProUGUI yearlyScoreText;
    public TextMeshProUGUI weeklyPositionText;
    public TextMeshProUGUI monthlyPositionText;
    public TextMeshProUGUI yearlyPositionText;

    private void Awake()
    {
        _instance = this;
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
    }

    public void ShowCountdown(float duration)
    {
        if (countdownPanel)
        {
            countdownPanel.SetActive(true);
            StartCoroutine(UpdateCountdownText(duration));
        }
    }

    private IEnumerator UpdateCountdownText(float duration)
    {
        int count = Mathf.CeilToInt(duration);

        while (count > 0)
        {
            countdownText.text = count.ToString();

            // Effetto di animazione del testo (opzionale)
            countdownText.transform.localScale = Vector3.one * 1.5f;
            float elapsedTime = 0;
            float scaleDuration = 0.5f;

            while (elapsedTime < scaleDuration)
            {
                float t = elapsedTime / scaleDuration;
                countdownText.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            count--;
            yield return new WaitForSeconds(0.5f); // Attendi il resto del secondo
        }

        // Mostra "GO!" alla fine del countdown
        countdownText.text = "GO!";
        countdownText.transform.localScale = Vector3.one * 1.3f;

        // Fai scomparire gradualmente "GO!"
        yield return new WaitForSeconds(0.5f);
        float fadeTime = 0;
        float fadeDuration = 0.5f;
        Color originalColor = countdownText.color;

        while (fadeTime < fadeDuration)
        {
            float t = fadeTime / fadeDuration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1, 0, t);
            countdownText.color = newColor;
            fadeTime += Time.deltaTime;
            yield return null;
        }

        // Ripristina l'alpha del testo per usi futuri
        Color resetColor = countdownText.color;
        resetColor.a = 1;
        countdownText.color = resetColor;

        // Nascondi il pannello
        countdownPanel.SetActive(false);
    }

    public void HideCountdown()
    {
        if (countdownPanel != null)
            countdownPanel.SetActive(false);
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
        finalScoreText.text =
            $"<color=#FF8B00>Final Score:</color> <color=#FFFFFF>{Mathf.FloorToInt(GameplayManager.Instance.score):D8}</color>";
        SaveHighScore();
        if (GameManager.Instance.isTournamentMode)
        {
            int currentScore = Mathf.FloorToInt(GameplayManager.Instance.score);
            PlayFabManager.Instance.SubmitScore(currentScore);
        }
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

        highScoreText.text =
            $"<color=#FFFFFF>High Score:</color> <color=#FF8B00> {PlayerPrefs.GetInt("HighScore"):D8}</color>";
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