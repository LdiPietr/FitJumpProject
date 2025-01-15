using UnityEngine;

public class SpawnWeightManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnWeight
    {
        public string name;
        public GameObject prefab;
        public float baseWeight;
        public float minWeight;
        public float maxWeight;
        public AnimationCurve difficultyCurve;
    }

    [Header("Platform Weights")]
    public SpawnWeight normalPlatform;
    public SpawnWeight movingPlatform;
    public SpawnWeight breakablePlatform;
    public SpawnWeight disappearingPlatform;

    [Header("PowerUp Weights")]
    public SpawnWeight jetpackPowerUp;
    public SpawnWeight shieldPowerUp;
    public SpawnWeight springPowerUp;

    [Header("Enemy Weights")]
    public SpawnWeight basicEnemy;
    public SpawnWeight movingEnemy;
    public SpawnWeight shootingEnemy;

    [Header("Base Weights")]
    public float basePlatformWeight = 100f;
    public float basePowerUpWeight = 5f;
    public float baseEnemyWeight = 3f;

    [Header("Difficulty Scaling")]
    public float maxDifficulty = 10f;
    public AnimationCurve powerUpScaling;
    public AnimationCurve enemyScaling;

    public float GetCurrentWeight(string type)
    {
        float difficulty = Mathf.Clamp01(GameplayManager.Instance.difficulty / maxDifficulty);
        
        switch(type)
        {
            case "platform": return basePlatformWeight;
            case "powerup": return basePowerUpWeight * powerUpScaling.Evaluate(difficulty);
            case "enemy": return baseEnemyWeight * enemyScaling.Evaluate(difficulty);
            default: return 1f;
        }
    }

    public float GetSpecificWeight(SpawnWeight spawnWeight)
    {
        float difficulty = Mathf.Clamp01(GameplayManager.Instance.difficulty / maxDifficulty);
        float evaluatedWeight = spawnWeight.baseWeight * spawnWeight.difficultyCurve.Evaluate(difficulty);
        return Mathf.Clamp(evaluatedWeight, spawnWeight.minWeight, spawnWeight.maxWeight);
    }
}