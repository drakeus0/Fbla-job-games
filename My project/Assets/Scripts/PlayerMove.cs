using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    private CharacterController controller;
    private Vector2 moveInput;
    private float yVelocity = 0f;
    private float yaw;
    private float pitch;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        // -------------------
        // Mouse Look
        // -------------------
        Vector2 mouseDelta = Vector2.zero;
        if (Mouse.current != null)
        {
            mouseDelta.x = Mouse.current.delta.x.ReadValue();
            mouseDelta.y = Mouse.current.delta.y.ReadValue();
        }

        transform.rotation = Quaternion.Euler(0, yaw, 0);

        moveInput.x = Keyboard.current.dKey.isPressed ? 1 : Keyboard.current.aKey.isPressed ? -1 : 0;
        moveInput.y = Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 velocity = move * moveSpeed;

        // -------------------
        // Gravity & Jump
        // -------------------
        if (controller.isGrounded)
        {
            yVelocity = -1f;
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
        }

        velocity.y = yVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}
