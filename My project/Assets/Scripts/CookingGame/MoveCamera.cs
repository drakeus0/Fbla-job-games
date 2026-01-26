using UnityEngine;
using UnityEngine.InputSystem;

public class MoveCamera : MonoBehaviour
{
    [Header("Camera Targets")]
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public float smoothSpeed = 5f; 

    private Transform currentTarget;

    private void Start()
    {
        currentTarget = pointA;
        transform.position = currentTarget.position;
    }

    private void Update()
    {
        HandleInput();
        SmoothMove();
    }

    private void HandleInput()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            currentTarget = pointA;
        }
        else if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentTarget = pointB;
        }
    }

    private void SmoothMove()
    {
        transform.position = Vector3.Lerp(transform.position, currentTarget.position, Time.deltaTime * smoothSpeed);
    }
}
