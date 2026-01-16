using UnityEngine;

public class RingManager : MonoBehaviour
{
    public static RingManager Instance;
    public Transform currentRing;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetRing(Transform ring)
    {
        currentRing = ring;
    }

    public void ClearRing(Transform ring)
    {
        if (currentRing == ring)
            currentRing = null;
    }
}
