using UnityEngine;

public class CloudAndRingSpawner : MonoBehaviour
{
    public Transform plane;              
    public GameObject[] cloudPrefabs;    
    public GameObject ringPrefab;

    public float spawnAheadDistance = 300f; // How far ahead clouds spawn
    public float horizontalSpread = 150f;   // Side-to-side randomness
    public float verticalSpread = 80f;      // Up/down randomness

    public float cloudSpawnInterval = 1.5f;
    public float cloudMinScale = 5f;
    public float cloudMaxScale = 10f;
    public float ringSpawnInterval = 4f;

    private float cloudTimer = 0f;
    private float ringTimer = 0f;

    void Update()
    {
        cloudTimer += Time.deltaTime;
        ringTimer += Time.deltaTime;

        if (cloudTimer >= cloudSpawnInterval)
        {
            SpawnCloud();
            cloudTimer = 0f;
        }

        if (ringTimer >= ringSpawnInterval)
        {
            SpawnRing();
            ringTimer = 0f;
        }
    }

    void SpawnCloud()
{
    if (cloudPrefabs.Length == 0) return;

    // Pick a random cloud prefab
    GameObject prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];

    // Base position = far ahead of the plane
    Vector3 spawnPos = plane.position + plane.forward * spawnAheadDistance;

    // Add side & vertical randomness
    spawnPos += plane.right * Random.Range(-horizontalSpread, horizontalSpread);
    spawnPos += plane.up * Random.Range(-verticalSpread, verticalSpread);

    // Spawn the cloud
    GameObject cloud = Instantiate(prefab, spawnPos, Quaternion.identity);

    // ----- ROTATION -----
    float randomX = Random.Range(-100f, -80f);
    float randomZ = Random.Range(-100f, -80f);
    float randomY = Random.Range(0f, 360f); // Full spin for variety

    cloud.transform.rotation = Quaternion.Euler(randomX, randomY, randomZ);

    // ----- SCALE -----
    float scale = Random.Range(cloudMinScale, cloudMaxScale);   // Adjust if needed
    cloud.transform.localScale = new Vector3(scale, scale, scale);
}


    void SpawnRing()
    {
        if (ringPrefab == null) return;

        // Spawn rings a bit above the cloud layer
        Vector3 spawnPos = plane.position + plane.forward * spawnAheadDistance;

        spawnPos += plane.right * Random.Range(-horizontalSpread, horizontalSpread);
        spawnPos += plane.up * Random.Range(-verticalSpread, verticalSpread);

        Instantiate(ringPrefab, spawnPos, Quaternion.identity);
    }
}
