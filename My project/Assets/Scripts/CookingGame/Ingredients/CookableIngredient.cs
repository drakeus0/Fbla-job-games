using System.Collections.Generic;
using UnityEngine;

public class CookableIngredient : MonoBehaviour
{
    private Rigidbody rb;
    private DraggableIngredientt draggable;

    private HashSet<CookItem> stovesTouching = new HashSet<CookItem>();
    private CookItem currentStove;
    private bool isCooking;

    public CookState cookState = CookState.Raw;
    public float cookTimer;

    [SerializeField] private Collider triggerCollider; // assign the trigger child here
    [SerializeField] private float knockOffImpulse = 2.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        draggable = GetComponent<DraggableIngredientt>();
    }

    private void OnEnable()
    {
        if (draggable == null) return;
        draggable.Dropped += OnDropped;
        draggable.PickedUp += OnPickedUp;
    }

    private void OnDisable()
    {
        if (draggable == null) return;
        draggable.Dropped -= OnDropped;
        draggable.PickedUp -= OnPickedUp;
    }

    private void OnPickedUp()
    {
        if (!isCooking) return;
        Debug.Log("Stopped cooking (picked up)");
        StopCookingInternal();
    }

    private void OnDropped()
    {
        // If we are touching any stove, start cooking on one of them
        if (!isCooking && stovesTouching.Count > 0)
        {
            // Pick the first stove we are touching
            foreach (var stove in stovesTouching)
            {
                StartCookingInternal(stove);
                break;
            }
        }
    }

    private void StartCookingInternal(CookItem stove)
    {
        isCooking = true;
        currentStove = stove;

        // Lock physics
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Snap to cook point
        transform.position = stove.CookPoint.position;
        transform.rotation = stove.CookPoint.rotation;

        stove.CookFood(this);
    }

    private void StopCookingInternal()
    {
        if (!isCooking) return;

        isCooking = false;

        if (currentStove != null)
        {
            currentStove.StopCooking();
            currentStove = null;
        }

        // Unfreeze all physics
        rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
        rb.constraints &= ~RigidbodyConstraints.FreezePositionX;
        rb.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
        rb.useGravity = true;
    }

    public void KnockOffStove(Vector3 forceDirection)
    {
        if (!isCooking) return;

        StopCookingInternal();
        rb.AddForce(forceDirection * knockOffImpulse, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("CookingGameStove")) return;

        CookItem stove = other.GetComponent<CookItem>();
        if (stove == null) return;

        stovesTouching.Add(stove);

        // Optionally, start cooking immediately if dropped on stove
        if (!isCooking && draggable != null && !draggable.isDraggingItem)
        {
            StartCookingInternal(stove);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("CookingGameStove")) return;

        CookItem stove = other.GetComponent<CookItem>();
        if (stove == null) return;

        stovesTouching.Remove(stove);

        // Stop cooking if we are no longer on any stove
        if (isCooking && stove == currentStove && stovesTouching.Count == 0)
        {
            StopCookingInternal();
        }
    }
}
