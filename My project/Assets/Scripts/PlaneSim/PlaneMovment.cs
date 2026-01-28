using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlaneMovement : MonoBehaviour
{
    /* =======================
       FLIGHT SPEEDS
       ======================= */
    [Header("Flight Speeds")]
    public float minFlySpeed = 10f;
    public float maxFlySpeed = 35f;
    float currentSpeed;

    /* =======================
       ENERGY
       ======================= */
    [Header("Energy")]
    public float energy = 100f;
    public float maxEnergy = 100f;
    public float energyDrainRate = 6f;
    public float rechargeAmount = 40f;
    bool energyDraining;

    /* =======================
       INPUT
       ======================= */
    float horizontalInput;
    float verticalInput;
    [SerializeField] GameObject flightControlsPrompt;
    [SerializeField] float flightPromptDuration = 5f;
    bool flightPromptShown = false;

    /* =======================
       ROTATION
       ======================= */
    [Header("Rotation")]
    public float yawSpeed = 90f;
    public float pitchSpeed = 70f;
    public float rollAngle = 35f;
    public float maxPitchAngle = 30f;

    float yawAngle;
    float pitchAngle;
    float yawVelocity;
    float pitchVelocity;
    float rollAngleCurrent;
    float rollVelocity;

    /* =======================
       TAKEOFF
       ======================= */
    [Header("Takeoff")]
    public float takeoffSpeed = 48f;
    public float takeoffPitchAngle = 26f;
    public float takeoffHeight = 45f;
    public GameObject takeoffText; // assign in inspector

    /* =======================
       FLIGHT STATE
       ======================= */
    public enum FlightState { Grounded, TakingOff, Flying }
    public FlightState flightState = FlightState.Grounded;

    IEnumerator ShowFlightControlsPrompt()
    {
        flightControlsPrompt.SetActive(true);
        yield return new WaitForSeconds(flightPromptDuration);
        flightControlsPrompt.SetActive(false);
    }

    /* =======================
       START
       ======================= */
    void Start()
    {
        currentSpeed = 0f;
        energyDraining = false;

        if (takeoffText != null)
            takeoffText.SetActive(true);
    }

    /* =======================
       UPDATE
       ======================= */
    void Update()
    {
        bool takeoffHeld = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;

        switch (flightState)
        {
            case FlightState.Grounded: HandleGrounded(takeoffHeld); return;
            case FlightState.TakingOff: HandleTakeoff(); return;
            case FlightState.Flying: HandleFlying(); break;
        }
    }

    /* =======================
       GROUNDED
       ======================= */
    void HandleGrounded(bool takeoffHeld)
    {
        currentSpeed = 0f;

        if (takeoffHeld)
        {
            flightState = FlightState.TakingOff;

            if (takeoffText != null)
                takeoffText.SetActive(false);
        }
    }

    /* =======================
       TAKEOFF
       ======================= */
    void HandleTakeoff()
    {
        // Accelerate along runway
        currentSpeed += 45f * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, takeoffSpeed);

        // Nose UP (negative pitch = up)
        pitchAngle = Mathf.MoveTowards(
            pitchAngle,
            -takeoffPitchAngle,
            60f * Time.deltaTime
        );

        transform.localRotation = Quaternion.Euler(
            pitchAngle,
            yawAngle,
            0f
        );

        // Forward motion + lift
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        // Become airborne
        if (transform.position.y >= takeoffHeight)
        {
            flightState = FlightState.Flying;
            energyDraining = true;
        }
    }

    /* =======================
       FLYING
       ======================= */
    void HandleFlying()
    {
        // Only show prompt once
        if (!flightPromptShown)
        {
            StartCoroutine(ShowFlightControlsPrompt());
            flightPromptShown = true;
        }

        /* =======================
           INPUT
           ======================= */
        horizontalInput = 0f;
        verticalInput = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            horizontalInput = -1f;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            horizontalInput = 1f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            verticalInput = 1f;
        else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            verticalInput = -1f;

        /* =======================
           ENERGY & SPEED
           ======================= */
        if (energyDraining)
        {
            energy -= energyDrainRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0f, maxEnergy);
        }

        float targetSpeed = Mathf.Lerp(minFlySpeed, maxFlySpeed, energy / maxEnergy);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 2f * Time.deltaTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        /* =======================
           PITCH
           ======================= */
        pitchVelocity += verticalInput * pitchSpeed * Time.deltaTime;
        pitchVelocity = Mathf.Lerp(pitchVelocity, 0f, 3f * Time.deltaTime);
        pitchAngle += pitchVelocity * Time.deltaTime;
        pitchAngle = Mathf.Clamp(pitchAngle, -maxPitchAngle, maxPitchAngle);

        /* =======================
           ROLL
           ======================= */
        float targetRoll = -horizontalInput * rollAngle;
        rollAngleCurrent = Mathf.SmoothDamp(rollAngleCurrent, targetRoll, ref rollVelocity, 0.6f);

        /* =======================
           YAW FROM ROLL (FIXED DIRECTION)
           ======================= */
        yawAngle += -rollAngleCurrent * 0.25f * Time.deltaTime; // <-- negative fixes direction

        /* =======================
           APPLY ROTATION
           ======================= */
        transform.localRotation = Quaternion.Euler(
            pitchAngle,
            yawAngle,
            rollAngleCurrent
        );
    }

    /* =======================
       RINGS
       ======================= */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ring"))
        {
            energy += rechargeAmount;
            energy = Mathf.Clamp(energy, 0f, maxEnergy);
        }
    }
}
