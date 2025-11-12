using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneMovement : MonoBehaviour
{
    public float FlySpeed = 20;
    public float YawAmount = 60;
    public float energy = 100f;           
    public float energyDrainRate = 5f;    
    public float minEnergy = 0f;          
    public float maxEnergy = 100f;        
    public float rechargeAmount = 50f;    
    public float maxFlySpeed = 20f;       
    public float minFlySpeed = 2f;        
    public float inputSmoothSpeed = 5f;   
    public float divePitchSpeed = 60f;    
    public float diveAcceleration = 30f;   

    private float Yaw;
    private float horizontalInput = 0f;
    private float verticalInput = 0f;
    private float diveSpeed = 0f;

    private bool horizontalUnlocked = false; // Tracks if horizontal movement is unlocked
    private bool energyDraining = false;     // Tracks if energy should start draining

    void Update()
    {
        // Unlock horizontal movement & start energy drain once Y >= 100
        if (!horizontalUnlocked && transform.position.y >= 100f)
        {
            horizontalUnlocked = true;
            energyDraining = true;
        }

        // Drain energy only after takeoff
        if (energyDraining && energy > minEnergy)
        {
            energy -= energyDrainRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, minEnergy, maxEnergy);
        }

        // --- Input ---
        float targetHorizontal = 0f;
        float targetVertical = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            targetHorizontal = -1f;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            targetHorizontal = 1f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            targetVertical = 1f;
        else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            targetVertical = -1f;

        horizontalInput = Mathf.MoveTowards(horizontalInput, targetHorizontal, inputSmoothSpeed * Time.deltaTime);
        verticalInput = Mathf.MoveTowards(verticalInput, targetVertical, inputSmoothSpeed * Time.deltaTime);

        // --- Movement ---
        if (transform.position.y > 0f)
        {
            if (energy > minEnergy)
            {
                float currentFlySpeed = Mathf.Lerp(minFlySpeed, maxFlySpeed, energy / maxEnergy);
                transform.position += transform.forward * currentFlySpeed * Time.deltaTime;

                // Rotation
                if (horizontalUnlocked)
                    Yaw += horizontalInput * YawAmount * Time.deltaTime;

                float pitch = Mathf.Lerp(0, 20, Mathf.Abs(verticalInput)) * Mathf.Sign(verticalInput);
                float roll = horizontalUnlocked ? Mathf.Lerp(0, 30, Mathf.Abs(horizontalInput)) * -Mathf.Sign(horizontalInput) : 0f;

                transform.localRotation = Quaternion.Euler(Vector3.up * Yaw + Vector3.right * pitch + Vector3.forward * roll);

                diveSpeed = currentFlySpeed;
            }
            else
            {
                // Free fall
                diveSpeed += diveAcceleration * Time.deltaTime;
                transform.position += transform.forward * diveSpeed * Time.deltaTime;

                Vector3 diveRotation = new Vector3(90f, Yaw, 0f); 
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(diveRotation), divePitchSpeed * Time.deltaTime);
            }

            // Clamp ground
            if (transform.position.y < 0f)
                transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ring"))
        {
            energy += rechargeAmount;
            energy = Mathf.Clamp(energy, minEnergy, maxEnergy);
        }
    }
}
