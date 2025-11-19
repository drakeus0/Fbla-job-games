using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Pickup_dropoff : MonoBehaviour
{
    [SerializeField] List<GameObject> dropoff_Points;
    GameObject chosen_Dropoff;

    [SerializeField] TextMeshProUGUI countDown;

    [SerializeField] float Deliverytime;

    bool deliveryStatus = false;
    float deliveryTimer;

    public void Start()
    {
        deliveryTimer = Deliverytime;
    }

    private void Update()
    {
        deliveryTimer -= Time.deltaTime;
        if (deliveryStatus == true)
        {w
            countDown.text = deliveryTimer.ToString();
        }
        if ( deliveryTimer < 0 )
        {
            StopDelivery();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("PickUp") && deliveryStatus == false) {
            deliveryStatus = true;
            StartDelivery();
            Debug.Log("Delivery Started");
        }
        else if (collision.gameObject.CompareTag("DropOff") && deliveryStatus == true)
        {
            deliveryStatus = false;
            StopDelivery();
        }
    }

    void StartDelivery()
    {
        GameObject randomDropoff = dropoff_Points[UnityEngine.Random.Range(0, dropoff_Points.Count-1)];
        chosen_Dropoff = randomDropoff;

        randomDropoff.SetActive(true);
        countDown.gameObject.SetActive(true);
    }
    void StopDelivery()
    {
        chosen_Dropoff.SetActive(false);
        countDown.gameObject.SetActive(false);
    }

}
