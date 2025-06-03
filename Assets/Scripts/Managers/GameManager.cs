using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int score = 0;
    public bool isTournamentMode = false;
    public string userName;
    public bool loged;
    public int tickets = 0;
    public PlayerLeaderboardInfo PlayerLeaderboardInfo; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayerLeaderboardInfo = new PlayerLeaderboardInfo();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void StartGame(bool tournamentMode)
    {
        score = 0;
        isTournamentMode = tournamentMode;
        SceneManager.LoadScene(tournamentMode ? "GameplayTournament" : "GameplayTraining");
    }

    public void EndGame()
    {
        if (isTournamentMode)
        {
            SceneManager.LoadScene("Leaderboard");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void IncreaseScore(int points)
    {
        score += points;
    }
    
    void OnApplicationQuit()
    {
        // Ripristina il timeout dello sleep quando l'applicazione viene chiusa
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }

}