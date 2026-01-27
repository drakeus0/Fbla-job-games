using UnityEngine;
using System.Collections.Generic;

public class RingManager : MonoBehaviour
{
    public static RingManager Instance;

    List<Transform> rings = new List<Transform>();
    public Transform currentTarget;

    Transform plane;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetPlane(Transform p)
    {
        plane = p;
    }


    void Update()
    {
        CleanupAndUpdateTarget();
    }

    void CleanupAndUpdateTarget()
    {
        if (plane == null)
            return; // ← nothing to do yet

        // Remove destroyed rings
        rings.RemoveAll(r => r == null);

        // Sort by closest ring IN FRONT of the plane
        rings.Sort((a, b) =>
            (a.position.z - plane.position.z)
            .CompareTo(b.position.z - plane.position.z)
        );

        // Pick the first ring that is ahead
        currentTarget = null;
        foreach (Transform ring in rings)
        {
            if (ring.position.z > plane.position.z)
            {
                currentTarget = ring;
                break;
            }
        }
    }


    public void RegisterRing(Transform ring)
    {
        if (!rings.Contains(ring))
        {
            rings.Add(ring);
            CleanupAndUpdateTarget(); // 🔥 immediate update
        }
    }

    public void CompleteRing(Transform ring)
    {
        if (rings.Remove(ring))
            CleanupAndUpdateTarget(); // 🔥 immediate update
    }

    public void RemoveRing(Transform ring)
    {
        if (rings.Remove(ring))
            CleanupAndUpdateTarget();
    }
}
