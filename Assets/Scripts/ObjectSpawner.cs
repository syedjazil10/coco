using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] collectiblePrefabs;
    [SerializeField] private GameObject[] pickupPrefabs;
    [SerializeField] private GameObject[] obstaclePrefabs;

    [SerializeField] private Transform spawnPoint;

    [Header("Collectibles")]
    [SerializeField] private float collectibleSpawnInterval = 1.5f;
    [SerializeField] private float collectibleSpawnDistance = 10f;

    [Header("Pickups")]
    [SerializeField] private float pickupSpawnInterval = 10f;
    [SerializeField] private float pickupSpawnDistance = 15f;

    [Header("Obstacles")]
    [SerializeField] private float obstacleSpawnInterval = 3f;
    [SerializeField] private float obstacleSpawnDistance = 20f;

    [SerializeField] private float spawnRangeMin = -1.5f;
    [SerializeField] private float spawnRangeMax = 1.5f;

    [SerializeField] private float spawnOffsetY = 0.25f;

    private float nextCollectibleSpawnTime;
    private float nextPickupSpawnTime;
    private float nextObstacleSpawnTime;

    private void Start()
    {
        float initialSpawnDelay = 1.0f;
        nextCollectibleSpawnTime = Time.time + collectibleSpawnInterval + initialSpawnDelay;
        nextPickupSpawnTime = Time.time + pickupSpawnInterval + initialSpawnDelay;
        nextObstacleSpawnTime = Time.time + obstacleSpawnInterval + initialSpawnDelay;
    }

    private void Update()
    {
        if (IsBeyondFinishLine())
        {
            return; // Stop spawning objects if beyond the finish line
        }

        if (Time.time >= nextCollectibleSpawnTime)
        {
            SpawnCollectible();
            nextCollectibleSpawnTime = Time.time + collectibleSpawnInterval;
        }

        if (Time.time >= nextPickupSpawnTime)
        {
            SpawnPickup();
            nextPickupSpawnTime = Time.time + pickupSpawnInterval;
        }

        if (Time.time >= nextObstacleSpawnTime)
        {
            SpawnObstacle();
            nextObstacleSpawnTime = Time.time + obstacleSpawnInterval;
        }
    }

    private void SpawnCollectible()
    {
        int randomIndex = Random.Range(0, collectiblePrefabs.Length);
        GameObject prefab = collectiblePrefabs[randomIndex];

        SpawnObject(prefab, collectibleSpawnDistance);
    }

    private void SpawnPickup()
    {
        int randomIndex = Random.Range(0, pickupPrefabs.Length);
        GameObject prefab = pickupPrefabs[randomIndex];

        SpawnObject(prefab, pickupSpawnDistance);
    }

    private void SpawnObstacle()
    {
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject prefab = obstaclePrefabs[randomIndex];

        SpawnObject(prefab, obstacleSpawnDistance);
    }

    private void SpawnObject(GameObject prefab, float spawnDistance)
    {
        Vector3 spawnPosition = spawnPoint.position + spawnPoint.forward * spawnDistance;

        // Calculate the X-axis position within the desired spawn range
        float spawnPosX = Random.Range(spawnRangeMin, spawnRangeMax);

        // Set the Y-axis position based on the ground level (assuming it's at y = 0)
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(spawnPosition.x, 10f, spawnPosition.z), Vector3.down, out hit, 20f, LayerMask.GetMask("Ground")))
        {
            spawnPosition.y = hit.point.y + spawnOffsetY;
        }
        else
        {
            spawnPosition.y = spawnOffsetY; // If the ground is not found, use the default offset
        }

        // Ensure that the spawn position is always ahead of the current spawn point
        if (spawnPosition.z < spawnPoint.position.z)
        {
            spawnPosition.z = spawnPoint.position.z;
        }

        GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);

        spawnPoint = spawnedObject.transform;
    }

    private bool IsBeyondFinishLine()
    {
        GameObject finishLine = GameObject.FindGameObjectWithTag("Finish");
        if (finishLine != null)
        {
            return spawnPoint.position.z >= finishLine.transform.position.z;
        }
        else
        {
            Debug.LogWarning("Finish line not found!");
            return false;
        }
    }
}
