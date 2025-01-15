using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject[] backgroundPrefabs;
    public float backgroundHeight = 10f;

    private GameObject[] activeBackgrounds;
    private int currentBackgroundIndex = 0;

    void Start()
    {
        activeBackgrounds = new GameObject[2];
        SpawnBackground(0);
        SpawnBackground(1);
    }

    void Update()
    {
        float cameraY = Camera.main.transform.position.y;
        if (cameraY > activeBackgrounds[currentBackgroundIndex].transform.position.y)
        {
            RecycleBackground();
        }
    }

    void SpawnBackground(int index)
    {
        Vector3 spawnPosition = new Vector3(0, index * backgroundHeight, 10);
        activeBackgrounds[index] = Instantiate(backgroundPrefabs[Random.Range(0, backgroundPrefabs.Length)], spawnPosition, Quaternion.identity);
    }

    void RecycleBackground()
    {
        int oldIndex = currentBackgroundIndex;
        currentBackgroundIndex = (currentBackgroundIndex + 1) % 2;
        Vector3 newPosition = activeBackgrounds[currentBackgroundIndex].transform.position + Vector3.up * (backgroundHeight * 2);
        activeBackgrounds[oldIndex].transform.position = newPosition;
    }
}
