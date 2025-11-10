using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float FlySpeed = 20;
    public float YawAmount = 60;            // Reduced turning speed
    public float energy = 100f;           
    public float energyDrainRate = 5f;    
    public float minEnergy = 0f;          
    public float maxEnergy = 100f;        
    public float rechargeAmount = 50f;    
    public float maxFlySpeed = 20f;       
    public float minFlySpeed = 2f;        // Speed at zero energy for normal flight (unused in dive)
    public float inputSmoothSpeed = 5f;   
    public float divePitchSpeed = 60f;    
    public float diveAcceleration = 30f;   // How fast the plane speeds up during nose dive

    private float Yaw;
    private float horizontalInput = 0f;
    private float verticalInput = 0f;
    private float diveSpeed = 0f;          // Speed used during nose dive

    void Update()
    {
        // Drain energy
        if (energy > minEnergy)
            energy -= energyDrainRate * Time.deltaTime;
        energy = Mathf.Clamp(energy, minEnergy, maxEnergy);

        // Smoothed keyboard input
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

        // Handle movement
        if (transform.position.y > 0f)
        {
            if (energy > minEnergy)
            {
                // Normal flight
                float currentFlySpeed = Mathf.Lerp(minFlySpeed, maxFlySpeed, energy / maxEnergy);
                transform.position += transform.forward * currentFlySpeed * Time.deltaTime;

                // Rotation based on input
                Yaw += horizontalInput * YawAmount * Time.deltaTime;
                float pitch = Mathf.Lerp(0, 20, Mathf.Abs(verticalInput)) * Mathf.Sign(verticalInput);
                float roll = Mathf.Lerp(0, 30, Mathf.Abs(horizontalInput)) * -Mathf.Sign(horizontalInput);
                transform.localRotation = Quaternion.Euler(Vector3.up * Yaw + Vector3.right * pitch + Vector3.forward * roll);

                diveSpeed = currentFlySpeed; // reset dive speed if plane regains energy
            }
            else
            {
                // Free fall: nose straight down, accelerate
                diveSpeed += diveAcceleration * Time.deltaTime; // accelerate
                transform.position += transform.forward * diveSpeed * Time.deltaTime;

                // Nose down rotation (clamped)
                Vector3 diveRotation = new Vector3(90f, Yaw, 0f); 
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(diveRotation), divePitchSpeed * Time.deltaTime);
            }

            // Clamp Y so it doesn't go below ground
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
