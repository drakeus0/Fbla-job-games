using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public Transform player; 
    // public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        
        // Follow the player's position with the specified offset
        // Vector3 desiredPosition = player.position + offset;
        // transform.position = desiredPosition;

        // Keep the camera's x and y rotation, but lock the z-axis rotation
        Vector3 rotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
    }
}
