using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class CloudSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform plane;
    public GameObject[] cloudPrefabs;

    [Header("Spawn Region (rectangle centered on custom point)")]
    public float centerY = 50f;
    public float halfWidthX = 400f;
    public float halfHeightY = 400f;
    public float halfDepthZ = 750f;

    [Header("Spawning")]
    [Tooltip("How many clouds to spawn instantly at game start.")]
    public int initialClouds = 30;
    [Tooltip("Time between cloud spawns.")]
    public float spawnInterval = 0.5f;
    [Tooltip("Forward offset from plane where initial clouds appear.")]
    public float spawnStartOffsetZ = 550f;

    [Tooltip("How far ahead continuous clouds spawn (separate from initial batch).")]
    public float continuousSpawnOffsetZ = 1500f;

    [Header("Scale")]
    public float minScale = 07f;
    public float maxScale = 15f;

    [Header("Despawn")]
    public float despawnBuffer = 50f;

    float planeInitialX;
    Coroutine spawnRoutine;

    void Start()
    {
        if (plane == null)
        {
            Debug.LogError("[CloudSpawner] Plane reference is null.");
            enabled = false;
            return;
        }

        if (cloudPrefabs == null || cloudPrefabs.Length == 0)
        {
            Debug.LogError("[CloudSpawner] No cloud prefabs assigned.");
            enabled = false;
            return;
        }

        planeInitialX = plane.position.x;

        float startZ = plane.position.z + spawnStartOffsetZ;

        // INITIAL INSTANT CLOUDS
        for (int i = 0; i < initialClouds; i++)
            SpawnSingleAtZ(startZ);

        // START CONTINUOUS SPAWNING
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        while (true)
        {
            if (plane == null) yield break;

            // --- UPDATED LINE ---
            float z = plane.position.z + continuousSpawnOffsetZ;

            SpawnSingleAtZ(z);

            yield return wait;
        }
    }

    void SpawnSingleAtZ(float spawnZ)
    {
        float chosenZ = Random.Range(spawnZ - halfDepthZ, spawnZ + halfDepthZ);

        float chosenX = Random.Range(planeInitialX - halfWidthX, planeInitialX + halfWidthX);
        float chosenY = Random.Range(centerY - halfHeightY, centerY + halfHeightY);

        Vector3 spawnPos = new Vector3(chosenX, chosenY, chosenZ);
        Quaternion spawnRot = Quaternion.Euler(-90f, Random.Range(-40f, 40f), -90f);

        GameObject prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
        if (prefab == null) return;

        GameObject cloudInstance = Instantiate(prefab, spawnPos, spawnRot);
        cloudInstance.name = prefab.name + "_instance";

        float scl = Random.Range(minScale, maxScale);
        cloudInstance.transform.localScale = Vector3.one * scl;

        CloudBehavior b = cloudInstance.GetComponent<CloudBehavior>();
        if (b == null) b = cloudInstance.AddComponent<CloudBehavior>();

        b.plane = plane;
        b.fadeDuration = Mathf.Max(0.2f, b.fadeDuration);
        b.despawnBuffer = despawnBuffer;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (plane == null) return;
        Vector3 center = new Vector3(
            plane.position.x,
            centerY,
            plane.position.z + spawnStartOffsetZ
        );

        Vector3 size = new Vector3(halfWidthX * 2f, halfHeightY * 2f, halfDepthZ * 2f);

        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.12f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.8f);
        Gizmos.DrawWireCube(center, size);
    }
#endif
}
