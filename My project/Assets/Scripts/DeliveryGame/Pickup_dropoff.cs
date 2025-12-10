using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Pickup_dropoff : MonoBehaviour
{
    [SerializeField] List<GameObject> dropoff_Points;
    GameObject chosen_Dropoff;

    [SerializeField] GameObject pickup_Point;

    [SerializeField] TextMeshProUGUI countDown;

    [SerializeField] float Deliverytime;

    float packagesMade = 2;
    float stars = 0;

    bool deliveryStatus = false;
    float deliveryTimer;

    StarAnimate starScript;

    public void Start()
    {
        deliveryTimer = Deliverytime;
        starScript = FindFirstObjectByType<StarAnimate>();
    }

    private void Update()
    {
        if (deliveryStatus == true)
        {
            deliveryTimer -= Time.deltaTime;
            countDown.text = deliveryTimer.ToString("F2");
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
        }
        else if (collision.gameObject.CompareTag("DropOff") && deliveryStatus == true)
        {
            deliveryStatus = false;
            stars += 1;
            StopDelivery();
        }
    }

    void StartDelivery()
    {
        GameObject randomDropoff = dropoff_Points[UnityEngine.Random.Range(0, dropoff_Points.Count)];
        chosen_Dropoff = randomDropoff;

        pickup_Point.SetActive(false);
        randomDropoff.SetActive(true);
        countDown.transform.parent.gameObject.SetActive(true);
    }
    void StopDelivery()
    {
            pickup_Point.SetActive(true);
            deliveryTimer = Deliverytime;
            chosen_Dropoff.SetActive(false);
        countDown.transform.parent.gameObject.SetActive(false);
        packagesMade += 1;
        if (packagesMade == 3)
        {
            starScript.ShowUI(stars);
        }
    }

}
