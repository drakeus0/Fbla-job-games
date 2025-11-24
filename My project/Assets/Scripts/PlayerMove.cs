using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class TopDownPlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f; // How fast player faces movement direction
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    private CharacterController controller;
    private float yVelocity = 0f;

    //animator
    private Animator animator;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // --- Input ---
        Vector3 inputDir = Vector3.zero;
        if (Keyboard.current.wKey.isPressed) inputDir += Vector3.forward;
        if (Keyboard.current.sKey.isPressed) inputDir += Vector3.back;
        if (Keyboard.current.aKey.isPressed) inputDir += Vector3.left;
        if (Keyboard.current.dKey.isPressed) inputDir += Vector3.right;

        inputDir = inputDir.normalized; // Normalize for diagonal movement

        //check for player movement
        if (inputDir == Vector3.zero)
        {
            animator.SetBool("Walking", false);
        }
        else
        {
            animator.SetBool("Walking", true);
        }

        // --- Rotation: face movement direction ---
        if (inputDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(-inputDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // --- Movement ---
        Vector3 move = inputDir * moveSpeed;

        yVelocity += gravity * Time.deltaTime;
        move.y = yVelocity;

        controller.Move(move * Time.deltaTime);
    }
}
