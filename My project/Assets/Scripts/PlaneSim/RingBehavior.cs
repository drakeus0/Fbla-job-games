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
        // Missed ring → cleanly remove
        if (plane.position.z - transform.position.z > despawnBuffer)
        {
            RingManager.Instance.RemoveRing(transform);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Hit ring → advance immediately
        RingManager.Instance.CompleteRing(transform);
        Destroy(gameObject);
    }
}
