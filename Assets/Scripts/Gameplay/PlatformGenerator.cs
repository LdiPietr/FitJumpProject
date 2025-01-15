using UnityEngine;
using System.Collections.Generic;

public class PlatformGenerator : MonoBehaviour
{
    [System.Serializable]
    public class PlatformSpawnData
    {
        public GameObject prefab;
        public float spawnChance; // Percentuale di spawn (0-100)
    }

    public List<PlatformSpawnData> platformSpawnData;

    public int platformsToSpawn = 20; // Numero di piattaforme da generare inizialmente
    public float platformSpawnDistanceY = 3f; // Distanza verticale tra le piattaforme
    public float minX = -5f;
    public float maxX = 5f;

    public Vector3 lastPlatformPosition;
    private HashSet<float> occupiedXPositions = new HashSet<float>();

    private void Start()
    {
        lastPlatformPosition = transform.position;
        GenerateInitialPlatforms();
    }

    public void GenerateInitialPlatforms()
    {
        for (int i = 0; i < platformsToSpawn; i++)
        {
            SpawnPlatform();
        }
    }

    private void SpawnPlatform()
    {
        Vector3 spawnPosition = GetRandomPlatformPosition();

        // Ottieni un prefab casuale dalla lista platformSpawnData
        GameObject prefabToSpawn = GetRandomPlatformPrefab();

        // Controlla se il prefab è stato trovato
        if (prefabToSpawn == null)
        {
            Debug.LogError("Prefab non trovato!");
            return;
        }

        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        lastPlatformPosition = spawnPosition;
    }

    private Vector3 GetRandomPlatformPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool positionFound = false;

        while (!positionFound)
        {
            spawnPosition.y = lastPlatformPosition.y + platformSpawnDistanceY;
            spawnPosition.x = Random.Range(minX, maxX);

            // Controlla se la posizione X è libera (opzionale)
            if (!occupiedXPositions.Contains(spawnPosition.x))
            {
                positionFound = true;
                occupiedXPositions.Add(spawnPosition.x);
            }
        }

        return spawnPosition;
    }

    private GameObject GetRandomPlatformPrefab()
    {
        float randomValue = Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        foreach (var data in platformSpawnData)
        {
            cumulativeChance += data.spawnChance;
            if (randomValue <= cumulativeChance)
            {
                return data.prefab;
            }
        }

        Debug.LogWarning("Nessun prefab selezionato casualmente. Controlla le probabilità di spawn.");
        return null;
    }
}
