using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    public SpawnWeightManager weightManager;

    public float minGapY = 0.5f;
    public float maxGapY = 0.5f;
    public float minGapX = 0.5f;
    public float limitGapY = 4f;
    public float screenMinX;
    public float screenMaxX;
    public List<GameObject> activePlatforms;
    public int numberOfPlatformsToSpawn = 100;
    public float destroyBelowY = -14f;
    private Camera mainCamera;
    private GameObject lastPowerUpSpawned;

    private void Start()
    {
        mainCamera = Camera.main;
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        screenMinX = -cameraWidth / 2 + 1f;
        screenMaxX = cameraWidth / 2 - 1f;

        Vector3 startPlatformPosition = new Vector3(0, -3, 0);
        GameObject firstPlatform =
            Instantiate(weightManager.normalPlatform.prefab, startPlatformPosition, Quaternion.identity);
        activePlatforms.Add(firstPlatform);

        PlayerController.Instance.transform.position = startPlatformPosition + Vector3.up;

        SpawnPlatform(numberOfPlatformsToSpawn);
    }

    private void Update()
    {
        float cameraY = mainCamera.transform.position.y;

        if (activePlatforms.Count < numberOfPlatformsToSpawn - 10)
            SpawnPlatform(numberOfPlatformsToSpawn - activePlatforms.Count);

        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            if (!activePlatforms[i] || activePlatforms[i].transform.position.y < cameraY + destroyBelowY)
            {
                if (activePlatforms[i])
                    Destroy(activePlatforms[i]);
                activePlatforms.RemoveAt(i);
            }
        }
    }

    private void SpawnPlatform(int platformToSpawn)
    {
        maxGapY = minGapY + (GameplayManager.Instance.comboMultiplier) / 2;
        if (maxGapY > limitGapY) maxGapY = limitGapY;

        var positions = GetPlatformPositions(platformToSpawn);
        foreach (var position in positions)
        {
            var spawned = Instantiate(GetRandomPlatformType(), position, Quaternion.identity);
            activePlatforms.Add(spawned);
            TrySpawnExtra(spawned.transform.position);
        }
    }

    private List<Vector2> GetPlatformPositions(int platformToSpawn)
    {
        var positions = new List<Vector2>();
        var lastActivePlatform = activePlatforms.LastOrDefault()!.transform.position;
        var lastPos = lastActivePlatform;
        for (var i = 0; i < platformToSpawn; i++)
        {
            var posY = Random.Range(lastPos.y, lastPos.y + maxGapY);
            var posX = Random.Range(screenMinX - 0.5f, screenMaxX + 0.5f);
            if (SpawnCheck(new Vector2(posX, posY)))
            {
                positions.Add(new Vector2(posX, posY));
                lastPos = new Vector2(posX, posY);
            }
        }

        return positions;
    }

    private GameObject GetRandomPlatformType()
    {
        float totalWeight =
            weightManager.GetSpecificWeight(weightManager.normalPlatform) +
            weightManager.GetSpecificWeight(weightManager.movingPlatform) +
            weightManager.GetSpecificWeight(weightManager.breakablePlatform) +
            weightManager.GetSpecificWeight(weightManager.disappearingPlatform);

        float randomWeight = Random.Range(0f, totalWeight);
        float currentWeight = 0;

        GameObject platformToSpawn = weightManager.normalPlatform.prefab;
        currentWeight += weightManager.GetSpecificWeight(weightManager.normalPlatform);
        if (randomWeight > currentWeight)
        {
            currentWeight += weightManager.GetSpecificWeight(weightManager.movingPlatform);
            if (randomWeight <= currentWeight)
            {
                platformToSpawn = weightManager.movingPlatform.prefab;
            }
            else
            {
                currentWeight += weightManager.GetSpecificWeight(weightManager.breakablePlatform);
                if (randomWeight <= currentWeight)
                {
                    platformToSpawn = weightManager.breakablePlatform.prefab;
                }
                else
                {
                    platformToSpawn = weightManager.disappearingPlatform.prefab;
                }
            }
        }

        return platformToSpawn;
    }

    private bool SpawnCheck(Vector2 positionToSpawn)
    {
        foreach (var platform in activePlatforms)
        {
            if (platform == null) continue;

            float platformX = platform.transform.position.x;
            float platformY = platform.transform.position.y;

            float adjustedMinGapY = minGapY;
            float adjustedMinGapX = minGapX + (GameplayManager.Instance.consecutiveJumps) / 2 *
                GameplayManager.Instance.comboMultiplier;

            if (Mathf.Abs(platformY - positionToSpawn.y) < adjustedMinGapY &&
                Mathf.Abs(positionToSpawn.x - platformX) < adjustedMinGapX)
            {
                return false;
            }
        }

        return true;
    }


    private void TrySpawnExtra(Vector2 platformPosition)
    {
        float powerUpWeight = weightManager.GetCurrentWeight("powerup");
        float enemyWeight = weightManager.GetCurrentWeight("enemy");
        float totalWeight = powerUpWeight + enemyWeight;

        float chance = Random.Range(0f, 100f);
        if (chance < totalWeight)
        {
            if (Random.Range(0f, totalWeight) < powerUpWeight)
                SpawnPowerUp(platformPosition);
            else
                SpawnEnemy(platformPosition);
        }
    }

    private void SpawnPowerUp(Vector2 position)
    {
        if(GameplayManager.Instance.comboMultiplier < 1.3f) return;
        
        float totalWeight =
            weightManager.GetSpecificWeight(weightManager.jetpackPowerUp) +
            weightManager.GetSpecificWeight(weightManager.shieldPowerUp) +
            weightManager.GetSpecificWeight(weightManager.springPowerUp);

        float randomWeight;
        float currentWeight;
        GameObject powerUpToSpawn;

        do
        {
            randomWeight = Random.Range(0f, totalWeight);
            currentWeight = 0;

            powerUpToSpawn = weightManager.jetpackPowerUp.prefab;

            currentWeight += weightManager.GetSpecificWeight(weightManager.jetpackPowerUp);
            if (randomWeight > currentWeight)
            {
                currentWeight += weightManager.GetSpecificWeight(weightManager.shieldPowerUp);
                if (randomWeight <= currentWeight)
                    powerUpToSpawn = weightManager.shieldPowerUp.prefab;
                else
                    powerUpToSpawn = weightManager.springPowerUp.prefab;
            }
        } while (powerUpToSpawn == lastPowerUpSpawned);

        lastPowerUpSpawned = powerUpToSpawn;

        Instantiate(powerUpToSpawn, position + Vector2.up, Quaternion.identity);
    }

    private void SpawnEnemy(Vector2 position)
    {
        if(GameplayManager.Instance.comboMultiplier < 1.5f) return;
        
        float totalWeight =
            weightManager.GetSpecificWeight(weightManager.basicEnemy) +
            weightManager.GetSpecificWeight(weightManager.movingEnemy) +
            weightManager.GetSpecificWeight(weightManager.shootingEnemy);

        float randomWeight = Random.Range(0f, totalWeight);
        float currentWeight = 0;

        GameObject enemyToSpawn = weightManager.basicEnemy.prefab;

        currentWeight += weightManager.GetSpecificWeight(weightManager.basicEnemy);
        if (randomWeight > currentWeight)
        {
            currentWeight += weightManager.GetSpecificWeight(weightManager.movingEnemy);
            if (randomWeight <= currentWeight)
                enemyToSpawn = weightManager.movingEnemy.prefab;
            else
                enemyToSpawn = weightManager.shootingEnemy.prefab;
        }

        var positionToSpawn = position + Vector2.up;
        var spawned = Instantiate(enemyToSpawn, positionToSpawn, Quaternion.identity);
        foreach (var platform in activePlatforms)
        {
            float platformX = platform.transform.position.x;
            float platformY = platform.transform.position.y;

            if (Mathf.Abs(platformY) - Mathf.Abs(positionToSpawn.y) < minGapY &&
                Mathf.Abs(positionToSpawn.x) - Mathf.Abs(platformX) < 2)
            {
                var vector3 = spawned.transform.position;
                vector3.x = vector3.x > 2 ? (vector3.x - 3) : (vector3.x + 3);
                spawned.transform.position = vector3;
                break;
            }
        }
    }
}