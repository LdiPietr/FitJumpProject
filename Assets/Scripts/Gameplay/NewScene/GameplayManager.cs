using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    
    [Header("Managers")]
    public SpawnWeightManager spawnWeightManager;
    public ParticleController particleController;
    
    [Header("Game Settings")]
    public float score;
    public float difficulty;
    public bool isGameOver;
    public int consecutiveJumps;
    public float comboMultiplier = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        score = 0;
        difficulty = 1;
        isGameOver = false;
        consecutiveJumps = 0;
        comboMultiplier = 1f;
    }

    public void AddScore(int amount)
    {
        score += amount * comboMultiplier;
        InGameUIManager.Instance.UpdateScoreText();
    }

    public void IncreaseCombo()
    {
        consecutiveJumps++;
        comboMultiplier = 1f + (consecutiveJumps * 0.1f);
    }

    public void ResetCombo()
    {
        consecutiveJumps = 0;
        comboMultiplier = 1f;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        AudioManager.Instance.PlayGameOverSound();
        particleController.PlayEnemyDeathEffect(PlayerController.Instance.transform.position);
        InGameUIManager.Instance.ShowGameOver();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            UpdateDifficulty();
        }
    }

    private void UpdateDifficulty()
    {
        difficulty = 1 + (score / 1000);
    }
}