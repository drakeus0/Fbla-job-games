using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableIngredientt : MonoBehaviour
{
    private Rigidbody rb;
    private Camera mainCamera;
    private bool isDragging = false;

    // Cooking
    private bool touchingStove = false;
    private Vector3 cookSpotPos;
    private bool cooked = false;
    private bool beingCooked = false;
    private GameObject stove;

    public bool canPickUp = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandlePickup();
        HandleDrop();
        HandleDragging();
    }

    private void HandlePickup()
    {
        if (Mouse.current.leftButton.isPressed && !isDragging && canPickUp)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            // Use the collider's Raycast to avoid other triggers blocking it
            if (GetComponent<Collider>().Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (beingCooked)
                {
                    stove.GetComponent<CookItem>().StopCooking();
                    beingCooked = false;
                }
                PickUp();
            }
        }
    }

    private void HandleDrop()
    {
        if (!Mouse.current.leftButton.isPressed && isDragging)
        {
            isDragging = false;

            if (!touchingStove || cooked)
            {
                rb.isKinematic = false;

                // Check for overlaps and move above if inside another collider
                Collider[] overlaps = Physics.OverlapBox(
                    transform.position,
                    GetComponent<Collider>().bounds.extents * 0.9f,
                    transform.rotation
                );

                foreach (Collider col in overlaps)
                {
                    if (col.gameObject == gameObject) continue;

                    // Move the ingredient above the collider it is overlapping
                    Vector3 abovePos = col.bounds.max;
                    transform.position = new Vector3(transform.position.x, abovePos.y + 0.1f, transform.position.z);
                    break; // only adjust once
                }
            }
            else
            {
                // Stove logic
                if (!cooked)
                {
                    canPickUp = false;
                    transform.position = cookSpotPos;

                    CookItem cookScript = stove.GetComponent<CookItem>();
                    cooked = true;
                    if (cookScript != null)
                    {
                        cookScript.CookFood(gameObject); // CookItem handles cooking timer
                        beingCooked = true;
                    }
                }
            }
        }
    }

    private void HandleDragging()
    {
        if (!isDragging) return;

        Vector2 mouseScreenPos2D = Mouse.current.position.ReadValue();

        Vector3 mouseScreenPos = new Vector3(
            mouseScreenPos2D.x,
            mouseScreenPos2D.y,
            Mathf.Abs(mainCamera.transform.position.z - transform.position.z)
        );

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        // Keep z fixed
        mouseWorldPos.z = transform.position.z;

        transform.position = mouseWorldPos;
    }

    private void PickUp()
    {
        isDragging = true;
        rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CookingGameStove"))
        {
            CookItem cookItem = other.GetComponent<CookItem>();
            if (cookItem != null && !cookItem.cooking)
            {
                stove = other.gameObject;
                cookSpotPos = other.transform.position;
                touchingStove = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CookingGameStove"))
        {
            touchingStove = false;
        }
    }
}
