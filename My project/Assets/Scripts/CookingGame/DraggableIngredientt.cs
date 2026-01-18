using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class DraggableIngredientt : MonoBehaviour
{
    private static DraggableIngredientt currentlyDragged;

    public event Action PickedUp;
    public event Action Dropped;

    private Rigidbody rb;
    private Camera mainCamera;

    public bool isDraggingItem;

    [Header("Drag Settings")]
    [SerializeField] private float followStrength = 20f;
    [SerializeField] private float maxDragSpeed = 10f;
    [SerializeField] private float dragDrag = 10f;
    [SerializeField] private float dragAngularDrag = 5f;

    private float defaultDrag;
    private float defaultAngularDrag;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        defaultDrag = rb.linearDamping;
        defaultAngularDrag = rb.angularDamping;
    }

    private void Update()
    {
        HandlePickup();
        HandleDrop();
    }

    private void FixedUpdate()
    {
        HandleDraggingPhysics();
    }

    private void HandlePickup()
    {
        if (!Mouse.current.leftButton.isPressed || currentlyDragged != null || isDraggingItem)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (GetComponent<Collider>().Raycast(ray, out _, Mathf.Infinity))
            PickUp();
    }

    private void HandleDrop()
    {
        if (Mouse.current.leftButton.isPressed || currentlyDragged != this)
            return;

        isDraggingItem = false;
        currentlyDragged = null;

        rb.useGravity = true;
        rb.linearDamping = defaultDrag;
        rb.angularDamping = defaultAngularDrag;

        Dropped?.Invoke();
    }

    private void HandleDraggingPhysics()
    {
        if (!isDraggingItem)
            return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(
            new Vector3(
                mouseScreenPos.x,
                mouseScreenPos.y,
                Mathf.Abs(mainCamera.transform.position.z - transform.position.z)
            )
        );

        mouseWorldPos.z = rb.position.z;

        Vector3 direction = mouseWorldPos - rb.position;

        rb.linearVelocity = Vector3.ClampMagnitude(
            direction * followStrength,
            maxDragSpeed
        );
    }

    private void PickUp()
    {
        isDraggingItem = true;
        currentlyDragged = this;

        rb.useGravity = false;
        rb.linearDamping = dragDrag;
        rb.angularDamping = dragAngularDrag;

        PickedUp?.Invoke();
    }
}
