using UnityEngine;

public class RingBehavior : MonoBehaviour
{
    Transform plane;
    float despawnBuffer;

    public void Init(Transform p, float buffer)
    {
        plane = p;
        despawnBuffer = buffer;
    }

    void Update()
    {
        if (plane.position.z - transform.position.z > despawnBuffer)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == plane)
        {
            RingManager.Instance.ClearRing(transform);
            // 🔋 Energy refill, score, sound, etc.
            Destroy(gameObject);
        }
    }
}
