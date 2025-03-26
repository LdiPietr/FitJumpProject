using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int score = 0;
    public bool isTournamentMode = false;

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
            // Aggiorna il punteggio nel database
            // Mostra la classifica
            SceneManager.LoadScene("Leaderboard");
        }
        else
        {
            // Mostra il punteggio e offri opzioni per ricominciare o tornare al menu principale
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