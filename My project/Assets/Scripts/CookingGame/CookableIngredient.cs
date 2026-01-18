using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class CookableIngredient : MonoBehaviour
{
    private Rigidbody rb;
    private DraggableIngredientt draggable;

    private bool touchingStove;
    private Vector3 cookSpotPos;
    private CookItem stove;

    private bool isCooking;

    public CookState cookState = CookState.Raw;
    public float cookTimer = 0f;

    [SerializeField] private float knockOffForce = 2.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        draggable = GetComponent<DraggableIngredientt>();
    }

    private void OnEnable()
    {
        if (draggable == null)
            return;

        draggable.Dropped += OnDropped;
        draggable.PickedUp += OnPickedUp;
    }

    private void OnDisable()
    {
        if (draggable == null)
            return;

        draggable.Dropped -= OnDropped;
        draggable.PickedUp -= OnPickedUp;
    }

    private void OnPickedUp()
    {
        if (!isCooking)
            return;

        // Player intentionally removed it
        isCooking = false;

        if (stove != null)
            stove.StopCooking();

        rb.isKinematic = false;
        rb.useGravity = true;

        stove = null;
    }

    private void OnDropped()
    {
        if (!touchingStove || stove == null || stove.cooking)
            return;

        // Lock to stove
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        transform.position = cookSpotPos;

        stove.CookFood(this);
        isCooking = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isCooking)
            return;

        if (collision.relativeVelocity.magnitude >= knockOffForce)
        {
            KnockOffStove();
        }
    }

    private void KnockOffStove()
    {
        isCooking = false;

        if (stove != null)
            stove.StopCooking();

        rb.isKinematic = false;
        rb.useGravity = true;

        stove = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("CookingGameStove"))
            return;

        CookItem cookItem = other.GetComponent<CookItem>();
        if (cookItem != null && !cookItem.cooking)
        {
            stove = cookItem;
            cookSpotPos = other.transform.position;
            touchingStove = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("CookingGameStove"))
            return;

        touchingStove = false;

        if (!isCooking)
            stove = null;
    }
}
