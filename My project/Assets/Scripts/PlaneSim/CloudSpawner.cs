using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class CloudSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform plane;                    // assign player plane in Inspector
    public GameObject[] cloudPrefabs;          // cloud prefabs (will not be modified)
    
    [Header("Spawn Region (rectangle centered on custom point)")]
    [Tooltip("Rect center X is locked to plane's initial X at Start(). Y is adjustable here.")]
    public float centerY = 10f;                // inspector-adjustable Y of the rectangle center
    [Tooltip("Half width on X axis (total width = 2 * halfWidthX)")]
    public float halfWidthX = 75f;
    [Tooltip("Half height on Y axis (total height = 2 * halfHeightY)")]
    public float halfHeightY = 25f;
    [Tooltip("Half depth on Z axis (total depth = 2 * halfDepthZ). Each spawn is placed within this depth around spawnZBase.")]
    public float halfDepthZ = 10f;

    [Header("Spacing & Density")]
    public float initialSpawnDistance = 150f;  // distance between initial cluster clouds
    public float laterSpawnDistance = 300f;    // distance after plane reaches initial cluster
    [Tooltip("Number of initial closer clouds (spawned immediately using initialSpawnDistance).")]
    public int initialCloudCount = 6;
    [Tooltip("Time between successive spawns (seconds)")]
    public float spawnInterval = 1.2f;
    [Tooltip("This value is used for new spawns; can be adjusted at runtime to control density.")]
    public float currentSpawnDistance = 300f;

    [Header("Initial placement")]
    [Tooltip("Z offset added to plane.initial.z for where first spawns are placed (relative forward offset).")]
    public float spawnStartOffsetZ = 200f;

    [Header("Scale")]
    public float minScale = 0.8f;
    public float maxScale = 2.2f;

    [Header("Other")]
    [Tooltip("Buffer used by clouds to decide when they're behind the plane and should destroy themselves.")]
    public float despawnBuffer = 50f;

    // internal state
    float planeInitialX;
    float planeInitialZ;
    float nextSpawnZ;                         // base Z used for the next spawned rectangle center
    float initialClusterEndZ;                 // when plane passes this Z, we switch spacing
    bool switchedToLaterSpacing = false;
    Coroutine spawnRoutine;

    void Start()
    {
        if (plane == null)
        {
            Debug.LogError("[CloudSpawner] Plane reference is null. Disabling spawner.");
            enabled = false;
            return;
        }

        if (cloudPrefabs == null || cloudPrefabs.Length == 0)
        {
            Debug.LogError("[CloudSpawner] No cloud prefabs assigned. Disabling spawner.");
            enabled = false;
            return;
        }

        // store plane initial X (locked for rectangle center X) and initial Z
        planeInitialX = plane.position.x;
        planeInitialZ = plane.position.z;

        // initialize nextSpawnZ: the first spawn cluster is ahead of the plane
        nextSpawnZ = planeInitialZ + spawnStartOffsetZ;

        // Spawn initial cluster using initialSpawnDistance so player won't see them pop-in
        float zForInitial = nextSpawnZ;
        for (int i = 0; i < Mathf.Max(0, initialCloudCount); i++)
        {
            SpawnSingleAtZ(zForInitial, useInitialCluster: true);
            zForInitial += initialSpawnDistance;
        }

        // after we created the initial cluster, set nextSpawnZ to the Z after last initial
        nextSpawnZ = zForInitial;

        // record the end Z of the initial cluster: once plane reaches this, switch spacing
        initialClusterEndZ = zForInitial - (initialSpawnDistance * 0.5f); // conservative switching point

        // set currentSpawnDistance to inspector value (may be equal to laterSpawnDistance or overwritten by user)
        if (currentSpawnDistance <= 0f) currentSpawnDistance = laterSpawnDistance;

        // start spawn coroutine
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        var wait = new WaitForSeconds(spawnInterval);
        while (true)
        {
            // safety: if plane destroyed, stop spawning to avoid MissingReferenceException
            if (plane == null)
                yield break;

            // If plane has advanced past the initial cluster region and we haven't switched yet -> switch
            if (!switchedToLaterSpacing && plane.position.z >= initialClusterEndZ)
            {
                switchedToLaterSpacing = true;
                currentSpawnDistance = laterSpawnDistance;
            }

            // spawn one cloud rectangle centered at nextSpawnZ
            SpawnSingleAtZ(nextSpawnZ, useInitialCluster: false);

            // increment nextSpawnZ by the current spawn distance (user-adjustable)
            nextSpawnZ += Mathf.Max(0.01f, currentSpawnDistance); // avoid zero spacing accidentally

            yield return wait;
        }
    }

    void SpawnSingleAtZ(float spawnZBase, bool useInitialCluster)
    {
        // Ensure spawn is ahead of plane (positive Z relative to plane)
        float minAllowedZ = (plane != null) ? plane.position.z + 1f : spawnZBase - halfDepthZ;
        float chosenZ = Random.Range(spawnZBase - halfDepthZ, spawnZBase + halfDepthZ);
        chosenZ = Mathf.Max(chosenZ, minAllowedZ);

        float chosenX = Random.Range(planeInitialX - halfWidthX, planeInitialX + halfWidthX);
        float chosenY = Random.Range(centerY - halfHeightY, centerY + halfHeightY);

        Vector3 spawnPos = new Vector3(chosenX, chosenY, chosenZ);

        // rotation: X = -90, Z = -90, Y random between -40 and +40
        Quaternion spawnRot = Quaternion.Euler(-90f, Random.Range(-40f, 40f), -90f);


        // --------------------------------------------------------------------
        // NEW CODE INSERTED EXACTLY AS PROVIDED
        // --------------------------------------------------------------------
        // pick a non-null prefab safely
        GameObject prefab = null;

        for (int safety = 0; safety < cloudPrefabs.Length; safety++)
        {
            var p = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
            if (p != null)
            {
                prefab = p;
                break;
            }
        }

        // If still null → do not spawn and do not crash
        if (prefab == null)
        {
            Debug.LogError("[CloudSpawner] All prefabs are null or have been destroyed. Spawning paused.");
            return;
        }
        // --------------------------------------------------------------------


        // instantiate EXACTLY as requested
        GameObject cloudInstance = Instantiate(prefab, spawnPos, spawnRot);

        cloudInstance.name = prefab.name + "_instance";

        // Set scale (uniform)
        float scl = Random.Range(minScale, maxScale);
        cloudInstance.transform.localScale = Vector3.one * scl;

        // Add or get CloudBehavior on the instance only (safe)
        CloudBehavior b = cloudInstance.GetComponent<CloudBehavior>();
        if (b == null) b = cloudInstance.AddComponent<CloudBehavior>();

        // assign properties on behavior
        b.plane = plane;
        b.fadeDuration = Mathf.Max(0.01f, b.fadeDuration); // keep existing fallback
        b.despawnBuffer = despawnBuffer;
    }

    void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

#if UNITY_EDITOR
    // Visualize spawn rectangle for the current nextSpawnZ center in Editor
    void OnDrawGizmosSelected()
    {
        if (plane == null) return;
        Vector3 center = new Vector3(
            (Application.isPlaying ? planeInitialX : plane.position.x),
            centerY,
            (Application.isPlaying ? nextSpawnZ : plane.position.z + spawnStartOffsetZ)
        );

        Vector3 size = new Vector3(halfWidthX * 2f, halfHeightY * 2f, halfDepthZ * 2f);
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.12f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.8f);
        Gizmos.DrawWireCube(center, size);
    }
#endif
}
