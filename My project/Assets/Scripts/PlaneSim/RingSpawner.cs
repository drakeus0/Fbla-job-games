using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class RingSpawner : MonoBehaviour
{
    public Transform plane;
    public GameObject ringPrefab;

    [Header("Spawn")]
    public float spawnInterval = 1.2f;
    public float spawnOffsetZ = 300f;

    [Header("Movement Range")]
    public float maxOffsetX = 30f;
    public float maxOffsetY = 15f;
    public float centerY = 10f;

    [Header("Despawn")]
    public float despawnBuffer = 50f;

    Vector3 lastSpawnOffset;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        // WAIT until plane reaches Y = 100
        yield return new WaitUntil(() => plane.position.y >= 100f);

        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        while (true)
        {
            SpawnRing();
            yield return wait;
        }
    }


   void SpawnRing()
{
    float z = plane.position.z + spawnOffsetZ;

    // FULLY RANDOM X/Y
    float xOffset = Random.Range(-maxOffsetX, maxOffsetX);
    float yOffset = Random.Range(-maxOffsetY, maxOffsetY);

    Vector3 pos = new Vector3(
        plane.position.x + xOffset,
        centerY + yOffset,
        z
    );

    GameObject ring = Instantiate(ringPrefab, pos, Quaternion.identity);
    ring.AddComponent<RingBehavior>().Init(plane, despawnBuffer);
    
    RingManager.Instance.SetRing(ring.transform);
}

}
