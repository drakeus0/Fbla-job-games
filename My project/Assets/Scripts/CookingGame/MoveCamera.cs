using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class MoveCamera : MonoBehaviour
{
    [Header("Camera Targets")]
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public float smoothTime = 0.3f; // SmoothDamp time
    private Vector3 velocity = Vector3.zero;

    [Header("Customer Initialization")]
    [SerializeField] private GameObject customersParent;

    private bool initialChange = true;
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

            if (initialChange)
            {
                initialChange = false;

                List<SpawnCustomer> allCustomers = new List<SpawnCustomer>();

                foreach (Transform child in customersParent.transform)
                {
                    SpawnCustomer spawn = child.GetComponent<SpawnCustomer>();
                    if (spawn != null)
                    {
                        allCustomers.Add(spawn);
                    }
                }

                if (allCustomers.Count > 0)
                {
                    int guaranteedIndex = Random.Range(0, allCustomers.Count);
                    StartCoroutine(SpawnCustomerAfterDelay(allCustomers[guaranteedIndex], 1f));

                    for (int i = 0; i < allCustomers.Count; i++)
                    {
                        if (i == guaranteedIndex) continue;
                        float randomDelay = Random.Range(10f, 20f);
                        StartCoroutine(SpawnCustomerAfterDelay(allCustomers[i], randomDelay));
                    }
                }
            }
        }
    }

    private void SmoothMove()
    {
        transform.position = Vector3.SmoothDamp(transform.position, currentTarget.position, ref velocity, smoothTime);
    }

    private IEnumerator SpawnCustomerAfterDelay(SpawnCustomer customer, float delay)
    {
        yield return new WaitForSeconds(delay);
        customer.RemoveAndShowCustomer(true);
    }
}
