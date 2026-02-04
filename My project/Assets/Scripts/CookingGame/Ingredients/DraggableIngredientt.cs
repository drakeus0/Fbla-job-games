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

    private LayerMask IngredientLayer;
    public IngredientType ingredientType;

    [Header("Drag Settings")]
    [SerializeField] private float followStrength = 20f;
    [SerializeField] private float maxDragSpeed = 10f;
    [SerializeField] private float dragDrag = 10f;
    [SerializeField] private float dragAngularDrag = 5f;

    private float defaultDrag;
    private float defaultAngularDrag;

    [Header("Stick")]
    public GameObject hostObject;
    public bool isHost => hostObject == null || hostObject == gameObject;
    private float StickTimer = .7f;
    private float stickProgress = 0f;
    private DraggableIngredientt pendingStickTarget = null;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        defaultDrag = rb.linearDamping;
        defaultAngularDrag = rb.angularDamping;

        IngredientLayer = LayerMask.NameToLayer("CookingGameIngredient");
    }

    private void Update()
    {
        HandlePickup();
        HandleDrop();
        HandleStickTimer();
    }

    private void FixedUpdate()
    {
        HandleDraggingPhysics();
    }

    #region Dragging
    private void HandlePickup()
    {
        if (!Mouse.current.leftButton.isPressed || currentlyDragged != null || isDraggingItem)
            return;

        // Convert mouse to world point
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Mathf.Abs(mainCamera.transform.position.z - transform.position.z)));

        // Use a tiny overlap box to detect trigger colliders
        Collider[] hits = Physics.OverlapBox(
            worldPoint,
            Vector3.one * 0.01f,
            Quaternion.identity,
            LayerMask.GetMask("CookingGameIngredient")
        );
        if (hits.Length > 0)
        {
            DraggableIngredientt draggable = hits[0].GetComponent<DraggableIngredientt>();
            if (draggable != null)
            {
                if (draggable.isHost)
                    draggable.PickUp();
                else if (draggable.hostObject != null)
                    draggable.hostObject.GetComponent<DraggableIngredientt>().PickUp();
            }
        }
    }


    private void HandleDrop()
    {
        if (Mouse.current.leftButton.isPressed || currentlyDragged != this)
            return;

        isDraggingItem = false;
        currentlyDragged = null;
        if (rb != null)
        {
            rb.useGravity = true;
            rb.linearDamping = defaultDrag;
            rb.angularDamping = defaultAngularDrag;
        }

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
    #endregion

    #region Stick

    private GameObject GetTopHost(GameObject obj)
    {
        DraggableIngredientt other = obj.GetComponent<DraggableIngredientt>();

        if (other == null || other.hostObject == null)
            return obj;

        return other.hostObject;
    }

    public void NotifyCollisionEnter(Collision collision)
    {
        if (!isHost) return;
        if (!isDraggingItem) return;
        if (collision.gameObject.layer != IngredientLayer) return;

        pendingStickTarget = collision.gameObject.GetComponent<DraggableIngredientt>();
    }

    public void NotifyCollisionExit(Collision collision)
    {
        if (!isHost) return;
        if (collision.gameObject.layer != IngredientLayer) return;

        DraggableIngredientt other = collision.gameObject.GetComponent<DraggableIngredientt>();
        if (other == pendingStickTarget)
        {
            pendingStickTarget = null;
            stickProgress = 0f;
        }
    }

    private void HandleStickTimer()
    {
        if (!isDraggingItem || pendingStickTarget == null)
        {
            stickProgress = 0f;
            return;
        }

        stickProgress += Time.deltaTime;
        if (stickProgress >= StickTimer)
        {
            StickWith(pendingStickTarget);
            stickProgress = 0f;
            pendingStickTarget = null;
        }
    }

    public void StickWith(DraggableIngredientt other)
    {
        if (other == null) return;
        if (other.gameObject.layer != IngredientLayer) return;

        GameObject myHost = GetTopHost(gameObject);
        GameObject otherHost = GetTopHost(other.gameObject);

        if (myHost == otherHost) return;

        // Decide which is final host (smaller instance ID for consistency)
        GameObject finalHost = myHost.GetInstanceID() < otherHost.GetInstanceID() ? myHost : otherHost;
        GameObject childStack = finalHost == myHost ? otherHost : myHost;

        childStack.transform.SetParent(finalHost.transform);

        // Update host references recursively
        UpdateHostRecursive(childStack.transform, finalHost);

        // Update layers recursively based on stack order
        UpdateLayerRecursive(finalHost.transform, 2); 

        // Stick the child stack
        DraggableIngredientt childStackScript = childStack.GetComponent<DraggableIngredientt>();
        if (childStackScript != null)
            childStackScript.StickToHost(finalHost);

        // Disable cookable if in a stack
        var cookable = GetComponent<CookableIngredient>();
        if (cookable != null)
            cookable.enabled = false;

        // Set host reference
        DraggableIngredientt finalHostScript = finalHost.GetComponent<DraggableIngredientt>();
        if (finalHostScript != null)
            finalHostScript.hostObject = finalHost;
    }


    private void UpdateHostRecursive(Transform root, GameObject host)
    {
        DraggableIngredientt ing = root.GetComponent<DraggableIngredientt>();
        if (ing != null)
            ing.hostObject = host;

        foreach (Transform child in root)
            UpdateHostRecursive(child, host);
    }

    private void UpdateLayerRecursive(Transform root, int baseLayer)
    {
        if (!root.GetComponent<DraggableIngredientt>().isDraggingItem)
        {
            root.gameObject.GetComponent<SpriteRenderer>().sortingOrder = baseLayer;
        }

        int childCount = root.childCount;
        int childLayer = baseLayer + 1;
        foreach (Transform child in root)
        {
            UpdateLayerRecursive(child, childLayer);
            childLayer++;
            Debug.Log(childLayer);
        }
    }


    private void StickToHost(GameObject host)
    {
        transform.SetParent(host.transform);

        if (host == gameObject) return;

        isDraggingItem = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            Destroy(rb);

        var collisionScript = GetComponent<IngredientCollisionHost>();
        if (collisionScript != null)
            collisionScript.enabled = false;
    }





    #endregion
}
