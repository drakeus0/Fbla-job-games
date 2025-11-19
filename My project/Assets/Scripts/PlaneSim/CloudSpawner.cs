using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform plane;                    // Player plane
    public GameObject[] cloudPrefabs;         // Multiple cloud prefabs

    [Header("Spawn Settings")]
    public float spawnCenterY = 50f;          // Adjustable Y center for clouds
    private float spawnCenterX;               // Will be set to plane's initial X
    public float initialSpawnDistance = 150f; // <-- LOWERED for denser initial clouds
    public float laterSpawnDistance = 300f;   // <-- LOWERED for denser clouds later
    public float horizontalSpread = 150f;     // X axis spread
    public float verticalSpread = 50f;        // Y axis randomness
    public float spawnInterval = 2f;          // Time between cloud spawns
    public float minScale = 5f;
    public float maxScale = 10f;

    [Header("Rotation Settings")]
    public float minYRotation = -40f;
    public float maxYRotation = 40f;
    
    private float spawnTimer = 0f;
    private float nextSpawnZ = 0f;
    private float currentSpawnDistance;

    private bool hasReachedFirstCloud = false; // Tracks if plane passed initial clouds

    void Start()
    {
        // Set horizontal center to plane's initial X
        spawnCenterX = plane.position.x;

        currentSpawnDistance = initialSpawnDistance;
        nextSpawnZ = plane.position.z + currentSpawnDistance;
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnCloud();
            spawnTimer = 0f;
        }

        // After plane passes first cloud batch, increase spacing
        if (!hasReachedFirstCloud && plane.position.z >= nextSpawnZ - currentSpawnDistance)
        {
            hasReachedFirstCloud = true;
            currentSpawnDistance = laterSpawnDistance;
        }
    }

    void SpawnCloud()
{
    if (cloudPrefabs.Length == 0) return;

    GameObject prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];

    Vector3 spawnPos = new Vector3(
        spawnCenterX + Random.Range(-horizontalSpread, horizontalSpread),
        spawnCenterY + Random.Range(-verticalSpread, verticalSpread),
        nextSpawnZ
    );

    GameObject cloud = Instantiate(prefab, spawnPos, Quaternion.identity);

    // Rotation: X/Z fixed at -90, Y random
    cloud.transform.rotation = Quaternion.Euler(-90f, Random.Range(minYRotation, maxYRotation), -90f);

    // Scale
    float scale = Random.Range(minScale, maxScale);
    cloud.transform.localScale = new Vector3(scale, scale, scale);

    // Ensure plane reference is assigned
    CloudBehavior behavior = cloud.GetComponent<CloudBehavior>();
    if (behavior == null)
        behavior = cloud.AddComponent<CloudBehavior>();
    behavior.plane = plane;

    // Move next spawn forward by spacing
    nextSpawnZ += currentSpawnDistance;
}

}
