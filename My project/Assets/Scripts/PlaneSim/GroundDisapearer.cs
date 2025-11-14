using UnityEngine;

public class GroundDisappearer : MonoBehaviour
{
    public Transform plane;           // Your plane/player
    public GameObject groundParent;   // Empty parent containing all ground objects
    public float disappearHeight = 120f; // Height where ground disappears

    void Update()
    {
        if (plane.position.y >= disappearHeight)
        {
            // Hide ground completely
            groundParent.SetActive(false);
        }
        else
        {
            // Show ground again if plane descends
            groundParent.SetActive(true);
        }
    }
}
