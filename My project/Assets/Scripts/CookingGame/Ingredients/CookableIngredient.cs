using UnityEngine;

public class CookableIngredient : MonoBehaviour
{
    private Rigidbody rb;
    private DraggableIngredientt draggable;

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
        if (!isCooking)
            return;

        StopCookingInternal();
    }

    private void OnDropped()
    {
        if (currentStove == null || currentStove.cooking)
            return;

        StartCookingInternal();
    }

    private void StartCookingInternal()
    {
        isCooking = true;

        // Lock physics cleanly
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Snap to cook point
        transform.position = currentStove.CookPoint.position;
        transform.rotation = currentStove.CookPoint.rotation;

        currentStove.CookFood(this);
    }

    private void StopCookingInternal()
    {
        isCooking = false;

        if (currentStove != null)
            currentStove.StopCooking();

        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;

        currentStove = null;
    }

    public void KnockOffStove(Vector3 forceDirection)
    {
        if (!isCooking)
            return;

        StopCookingInternal();
        rb.AddForce(forceDirection * knockOffImpulse, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only care if the collision is from our trigger collide
        if (!other.CompareTag("CookingGameStove")) return;
        Debug.Log("Touching stove");
        currentStove = other.GetComponent<CookItem>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("CookingGameStove")) return;
        Debug.Log("NOT Touching stove");
        CookItem stove = other.GetComponent<CookItem>();
        if (stove == currentStove) currentStove = null;
    }

}
