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

    [SerializeField] float amountOfPackages;

    float packagesMade = 0;

    bool deliveryStatus = false;
    float deliveryTimer = 0f;

    [SerializeField] StarAnimate starScript;

    public void Start()
    {
        if (GameSettings.presentMode) amountOfPackages = 1;
    }

    private void Update()
    {
        if (deliveryStatus == true)
        {
            deliveryTimer += Time.deltaTime;
            countDown.text = deliveryTimer.ToString("F2");
        }
        if (packagesMade >= amountOfPackages)
        {
            deliveryStatus = false;
            float stars = (deliveryTimer <= 110f) ? 3 :
                    (deliveryTimer <= 130f) ? 2 :
                    (deliveryTimer <= 150f) ? 1 : 0;
            starScript.ShowUI(stars);
            if (stars > MainGameData.DeliveryHighScore) MainGameData.DeliveryHighScore = (int)stars;
            MainGameData.DeliveryCompleted = true;
            this.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("PickUp") && packagesMade < amountOfPackages) {
            deliveryStatus = true;
            StartDelivery();
        }
        else if (collision.gameObject.CompareTag("DropOff") && deliveryStatus == true)
        {
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
        if (chosen_Dropoff)
        {
        pickup_Point.SetActive(true);
        chosen_Dropoff.SetActive(false);
        packagesMade += 1; 
        }
    }

}
