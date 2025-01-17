using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public SpawnWeightManager weightManager;
    public float minX = -5f;
    public float maxX = 5f;
    public float verticalSpawnDistance = 1.2f; // Ridotto per aumentare densità
    public float maxHorizontalGap = 1.8f; // Massima distanza orizzontale tra piattaforme
    private Vector2 lastPlatformPosition;
    public float destroyBelowY = -14f;
    public float spawnAboveY = 4f;
    public float platformDensityMultiplier = 2f;

    private List<GameObject> activePlatforms = new List<GameObject>();
    private float lastSpawnY;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // Prima piattaforma e player
        Vector3 startPlatformPosition = new Vector3(0, -3, 0);
        GameObject firstPlatform =
            Instantiate(weightManager.normalPlatform.prefab, startPlatformPosition, Quaternion.identity);
        activePlatforms.Add(firstPlatform);

        PlayerController.Instance.transform.position = startPlatformPosition + Vector3.up;

        lastSpawnY = startPlatformPosition.y;

        // Piattaforme iniziali
        float currentY = lastSpawnY;
        for (int i = 0; i < 15; i++)
        {
            currentY += verticalSpawnDistance;
            Vector2 spawnPosition = new Vector2(Random.Range(minX, maxX), currentY);
            GameObject platform = Instantiate(weightManager.normalPlatform.prefab, spawnPosition, Quaternion.identity);
            activePlatforms.Add(platform);
        }

        lastSpawnY = currentY;
    }

    private void Update()
    {
        float cameraY = mainCamera.transform.position.y;

        // Spawn nuove piattaforme
        while (lastSpawnY < cameraY + spawnAboveY)
        {
            for (int i = 0; i < platformDensityMultiplier; i++)
            {
                SpawnPlatform();
            }
        }


        // Cleanup più aggressivo delle piattaforme
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

    private void SpawnPlatform()
    {
        // Calcola i limiti dello schermo
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;
    
        // Usa questi limiti per lo spawn
        float screenMinX = -cameraWidth/2 + 1f; // +1 per margine
        float screenMaxX = cameraWidth/2 - 1f;  // -1 per margine

        // Calcola posizione di spawn
        float newX;
        if (activePlatforms.Count > 0)
        {
            float minX = Mathf.Max(lastPlatformPosition.x - maxHorizontalGap, screenMinX);
            float maxX = Mathf.Min(lastPlatformPosition.x + maxHorizontalGap, screenMaxX);
            newX = Random.Range(minX, maxX);
        }
        else
        {
            newX = Random.Range(screenMinX, screenMaxX);
        }

        Vector2 spawnPosition = new Vector2(newX, lastSpawnY + verticalSpawnDistance);
        lastPlatformPosition = spawnPosition;

        // Calcola i pesi totali per le piattaforme
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
                platformToSpawn = weightManager.movingPlatform.prefab;
            else
            {
                currentWeight += weightManager.GetSpecificWeight(weightManager.breakablePlatform);
                if (randomWeight <= currentWeight)
                    platformToSpawn = weightManager.breakablePlatform.prefab;
                else
                    platformToSpawn = weightManager.disappearingPlatform.prefab;
            }
        }

        GameObject platform = Instantiate(platformToSpawn, spawnPosition, Quaternion.identity);
        activePlatforms.Add(platform);
        lastSpawnY = spawnPosition.y;

        // Chance di spawn power-up o nemico
        TrySpawnExtra(spawnPosition);
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
        float totalWeight =
            weightManager.GetSpecificWeight(weightManager.jetpackPowerUp) +
            weightManager.GetSpecificWeight(weightManager.shieldPowerUp) +
            weightManager.GetSpecificWeight(weightManager.springPowerUp);

        float randomWeight = Random.Range(0f, totalWeight);
        float currentWeight = 0;

        GameObject powerUpToSpawn = weightManager.jetpackPowerUp.prefab;

        currentWeight += weightManager.GetSpecificWeight(weightManager.jetpackPowerUp);
        if (randomWeight > currentWeight)
        {
            currentWeight += weightManager.GetSpecificWeight(weightManager.shieldPowerUp);
            if (randomWeight <= currentWeight)
                powerUpToSpawn = weightManager.shieldPowerUp.prefab;
            else
                powerUpToSpawn = weightManager.springPowerUp.prefab;
        }

        Instantiate(powerUpToSpawn, position + Vector2.up, Quaternion.identity);
    }

    private void SpawnEnemy(Vector2 position)
    {
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

        Instantiate(enemyToSpawn, position + Vector2.up, Quaternion.identity);
    }
}