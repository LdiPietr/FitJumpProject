using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem.iOS;

public class SpawnManager : MonoBehaviour
{
    public SpawnWeightManager weightManager;
    public float initialVerticalSpawnDistance = 0.8f;
    public float maxVerticalSpawnDistance = 10.0f;
    public float maxHorizontalGap = 1.8f;
    private Vector2 lastPlatformPosition;
    public float destroyBelowY = -14f;
    public float spawnAboveY = 4f;
    public float platformDensityMultiplier = 100;

    public List<GameObject> activePlatforms = new List<GameObject>();
    private float lastSpawnY;
    private Camera mainCamera;

    private Platform.PlatformType lastPlatformType = Platform.PlatformType.Normal;
    private GameObject lastPowerUpSpawned;

    private void Start()
    {
        mainCamera = Camera.main;

        Vector3 startPlatformPosition = new Vector3(0, -3, 0);
        GameObject firstPlatform =
            Instantiate(weightManager.normalPlatform.prefab, startPlatformPosition, Quaternion.identity);
        activePlatforms.Add(firstPlatform);

        PlayerController.Instance.transform.position = startPlatformPosition + Vector3.up;

        lastSpawnY = startPlatformPosition.y;

        float currentY = lastSpawnY;
        for (int i = 0; i < 30; i++)
        {
            SpawnPlatform();
        }

        lastSpawnY = currentY;
    }

    private void Update()
    {
        float cameraY = mainCamera.transform.position.y;

        while (lastSpawnY < cameraY + spawnAboveY)
        {
            for (int i = 0; i < platformDensityMultiplier; i++)
            {
                SpawnPlatform();
            }
        }

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
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float screenMinX = -cameraWidth / 2 + 1f;
        float screenMaxX = cameraWidth / 2 - 1f;

        // Calcola il gap orizzontale in base all'altezza
        float currentMaxHorizontalGap = CalculateHorizontalGap(mainCamera.transform.position.y);

        float newX;
        if (activePlatforms.Count > 0)
        {
            float minX = Mathf.Max(lastPlatformPosition.x - currentMaxHorizontalGap, screenMinX);
            float maxX = Mathf.Min(lastPlatformPosition.x + currentMaxHorizontalGap, screenMaxX);
            newX = Random.Range(minX, maxX);
        }
        else
        {
            newX = Random.Range(screenMinX, screenMaxX);
        }

        float verticalSpawnDistance = Mathf.Lerp(initialVerticalSpawnDistance, maxVerticalSpawnDistance,
            Mathf.Pow(lastSpawnY / 100f, 4));

        Vector2 spawnPosition = new Vector2(newX, lastSpawnY + verticalSpawnDistance);
        lastPlatformPosition = spawnPosition;

        float totalWeight =
            weightManager.GetSpecificWeight(weightManager.normalPlatform) +
            weightManager.GetSpecificWeight(weightManager.movingPlatform) +
            weightManager.GetSpecificWeight(weightManager.breakablePlatform) +
            weightManager.GetSpecificWeight(weightManager.disappearingPlatform);

        float randomWeight = Random.Range(0f, totalWeight);
        float currentWeight = 0;

        GameObject platformToSpawn = weightManager.normalPlatform.prefab;
        Platform.PlatformType platformTypeToSpawn = Platform.PlatformType.Normal;
        currentWeight += weightManager.GetSpecificWeight(weightManager.normalPlatform);
        if (randomWeight > currentWeight)
        {
            currentWeight += weightManager.GetSpecificWeight(weightManager.movingPlatform);
            if (randomWeight <= currentWeight)
            {
                platformToSpawn = weightManager.movingPlatform.prefab;
                platformTypeToSpawn = Platform.PlatformType.Moving;
            }
            else
            {
                currentWeight += weightManager.GetSpecificWeight(weightManager.breakablePlatform);
                if (randomWeight <= currentWeight)
                {
                    platformToSpawn = weightManager.breakablePlatform.prefab;
                    platformTypeToSpawn = Platform.PlatformType.Breakable;
                }
                else
                {
                    platformToSpawn = weightManager.disappearingPlatform.prefab;
                    platformTypeToSpawn = Platform.PlatformType.Disappearing;
                }
            }
        }

        // Evita di spawnare due piattaforme "Disappearing" di seguito
        if (platformTypeToSpawn == Platform.PlatformType.Disappearing &&
            lastPlatformType == Platform.PlatformType.Disappearing)
        {
            // Riprova a generare una piattaforma diversa
            SpawnPlatform();
            return;
        }

        SpawnRoutine(spawnPosition, platformToSpawn);
        lastSpawnY = spawnPosition.y;
        lastPlatformType = platformTypeToSpawn;
    }

    // Funzione per calcolare il gap orizzontale
    private float CalculateHorizontalGap(float currentY)
    {
        float minGap = 0.3f; // Gap minimo desiderato
        float maxGap = maxHorizontalGap; // Gap massimo originale
        float normalizedHeight = Mathf.Clamp01(currentY / 100f); // Normalizza l'altezza

        // Calcola il gap attuale
        return Mathf.Lerp(minGap, maxGap, normalizedHeight);
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

        lastPowerUpSpawned = powerUpToSpawn; // Aggiorna l'ultimo power-up generato

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

        var positionToSpawn = position + Vector2.up;
        var spawned = Instantiate(enemyToSpawn, positionToSpawn, Quaternion.identity);
        foreach (var platform in activePlatforms)
        {
            float platformX = platform.transform.position.x;
            float platformY = platform.transform.position.y;

            if (Mathf.Abs(platformY) - Mathf.Abs(positionToSpawn.y) < 0.5f &&
                Mathf.Abs(positionToSpawn.x) - Mathf.Abs(platformX) < 2)
            {
                var vector3 = spawned.transform.position;
                vector3.x = vector3.x > 2 ? (vector3.x - 3) : (vector3.x + 3);
                spawned.transform.position = vector3;
                break;
            }
        }
    }

    private void SpawnRoutine(Vector2 positionToSpawn, GameObject objectToSpawn)
    {
        foreach (var platform in activePlatforms)
        {
            float platformX = platform.transform.position.x;
            float platformY = platform.transform.position.y;

            if (Mathf.Abs(platformY - positionToSpawn.y) < 1f &&
                Mathf.Abs(positionToSpawn.x - platformX) < GameplayManager.Instance.comboMultiplier * 0.2f)
            {
                print(Mathf.Abs(platformY - positionToSpawn.y) + "   " +
                      Mathf.Abs(positionToSpawn.x - platformX));
                return;
            }
        }

        var spawned = Instantiate(objectToSpawn, positionToSpawn, Quaternion.identity);
        activePlatforms.Add(spawned);
    }
}