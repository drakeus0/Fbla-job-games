using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class PlayerMovement : MonoBehaviour
{
    public float FlySpeed = 5;
    public float YawAmount = 120;
    public float energy = 100f;           // Maximum energy
    public float energyDrainRate = 5f;    // Rate at which energy drains per second
    public float minEnergy = 0f;          // Minimum energy level
    public float maxEnergy = 100f;        // Maximum energy level
    public float rechargeAmount = 50f;    // Amount of energy regained when passing through a ring
    public float maxFlySpeed = 10f;       // Maximum speed of the plane
    public float minFlySpeed = 2f;        // Minimum speed of the plane
    public float inputSmoothSpeed = 5f;   // Speed at which input smooths (higher = faster response)

    private float Yaw;
    private float horizontalInput = 0f;
    private float verticalInput = 0f;

    void Update()
    {
        // Drain energy over time
        if (energy > minEnergy)
        {
            energy -= energyDrainRate * Time.deltaTime;
        }

        energy = Mathf.Clamp(energy, minEnergy, maxEnergy);

        // Calculate current speed based on energy
        float currentFlySpeed = Mathf.Lerp(minFlySpeed, maxFlySpeed, energy / maxEnergy);

        // Move the plane forward
        if (energy > minEnergy)
        {
            transform.position += transform.forward * currentFlySpeed * Time.deltaTime;
        }

        // --- Smoothed keyboard input ---
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

        // Smooth input like Input.GetAxis
        horizontalInput = Mathf.MoveTowards(horizontalInput, targetHorizontal, inputSmoothSpeed * Time.deltaTime);
        verticalInput = Mathf.MoveTowards(verticalInput, targetVertical, inputSmoothSpeed * Time.deltaTime);

        // Apply rotation based on smoothed input
        Yaw += horizontalInput * YawAmount * Time.deltaTime;
        float pitch = Mathf.Lerp(0, 20, Mathf.Abs(verticalInput)) * Mathf.Sign(verticalInput);
        float roll = Mathf.Lerp(0, 30, Mathf.Abs(horizontalInput)) * -Mathf.Sign(horizontalInput);

        transform.localRotation = Quaternion.Euler(Vector3.up * Yaw + Vector3.right * pitch + Vector3.forward * roll);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.gameObject.name);
        if (other.CompareTag("Ring"))
        {
            Debug.Log("Energy reset!");
            energy += rechargeAmount;
            energy = Mathf.Clamp(energy, minEnergy, maxEnergy);
        }
    }
}
