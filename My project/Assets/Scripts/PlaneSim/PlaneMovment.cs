using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlaneMovement : MonoBehaviour
{
    [Header("Flight Speeds")]
    public float minFlySpeed = 10f;
    public float maxFlySpeed = 35f;
    float currentSpeed;

    [Header("Energy")]
    public float energy = 100f;
    public float maxEnergy = 100f;
    public float energyDrainRate = 6f;
    public float rechargeAmount = 40f;
    bool energyDraining;

    float horizontalInput;
    float verticalInput;
    [SerializeField] GameObject flightControlsPrompt;
    [SerializeField] float flightPromptDuration = 5f;
    bool flightPromptShown = false;

    [Header("Rotation")]
    public float yawSpeed = 90f;
    public float pitchSpeed = 70f;
    public float rollAngle = 35f;
    public float maxPitchAngle = 30f;

    float yawAngle;
    float pitchAngle;
    float pitchVelocity;
    float rollAngleCurrent;
    float rollVelocity;

    [Header("Takeoff")]
    public float takeoffSpeed = 48f;
    public float takeoffPitchAngle = 26f;
    public float takeoffHeight = 45f;
    public GameObject takeoffText;

    [Header("Crash / Out of Energy")]
    public float outOfEnergyDivePitch = 55f;
    public float outOfEnergyForwardSpeed = 30f;
    public float crashStopY = 0f;

    bool outOfEnergy = false;
    float lockedDiveYaw;

    [Header("Crash Terrain Block")]
    public Terrain crashTerrain;          // drag CrashTerrain here
    public float crashTerrainY = 0f;      // usually 0
    public Vector3 crashTerrainOffset;    // e.g. (0,0,200) if you want it ahead
    public bool followPlaneWhileDiving = true;

    [Header("Disable plane collider during dive (prevents spin-outs)")]
    public bool disablePlaneColliderOnDive = true;

    public RunStatsUI runUI;

    Collider planeCollider;

    public enum FlightState { Grounded, TakingOff, Flying }
    public FlightState flightState = FlightState.Grounded;

    IEnumerator ShowFlightControlsPrompt()
    {
        flightControlsPrompt.SetActive(true);
        yield return new WaitForSeconds(flightPromptDuration);
        flightControlsPrompt.SetActive(false);
    }

    void Start()
    {
        currentSpeed = 0f;
        energyDraining = false;

        planeCollider = GetComponent<Collider>();

        if (takeoffText != null)
            takeoffText.SetActive(true);

        // Make sure crash terrain starts hidden (so you only see it when needed)
        if (crashTerrain != null)
            crashTerrain.gameObject.SetActive(false);
    }

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

    void HandleTakeoff()
    {
        currentSpeed += 45f * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, takeoffSpeed);

        pitchAngle = Mathf.MoveTowards(pitchAngle, -takeoffPitchAngle, 60f * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(pitchAngle, yawAngle, 0f);

        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        if (transform.position.y >= takeoffHeight)
        {
            flightState = FlightState.Flying;
            energyDraining = true;
            if (runUI) runUI.StartRun();
        }
    }

    void HandleFlying()
    {
        if (outOfEnergy)
        {
            HandleOutOfEnergyDive();
            return;
        }

        if (!flightPromptShown)
        {
            StartCoroutine(ShowFlightControlsPrompt());
            flightPromptShown = true;
        }

        horizontalInput = 0f;
        verticalInput = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontalInput = -1f;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontalInput = 1f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) verticalInput = 1f;
        else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) verticalInput = -1f;

        if (energyDraining)
        {
            energy -= energyDrainRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0f, maxEnergy);
        }

        float targetSpeed = Mathf.Lerp(minFlySpeed, maxFlySpeed, energy / maxEnergy);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 2f * Time.deltaTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        pitchVelocity += verticalInput * pitchSpeed * Time.deltaTime;
        pitchVelocity = Mathf.Lerp(pitchVelocity, 0f, 3f * Time.deltaTime);
        pitchAngle += pitchVelocity * Time.deltaTime;
        pitchAngle = Mathf.Clamp(pitchAngle, -maxPitchAngle, maxPitchAngle);

        float targetRoll = -horizontalInput * rollAngle;
        rollAngleCurrent = Mathf.SmoothDamp(rollAngleCurrent, targetRoll, ref rollVelocity, 0.6f);

        yawAngle += -rollAngleCurrent * 0.4f * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(pitchAngle, yawAngle, rollAngleCurrent);

        // Start dive
        if (energy <= 0f)
        {
            outOfEnergy = true;
            lockedDiveYaw = yawAngle;

            pitchVelocity = 0f;
            rollVelocity = 0f;

            if (disablePlaneColliderOnDive && planeCollider != null)
                planeCollider.enabled = false;

            EnableAndPlaceCrashTerrain();
        }
    }

    void HandleOutOfEnergyDive()
    {
        pitchAngle = Mathf.MoveTowards(pitchAngle, outOfEnergyDivePitch, 80f * Time.deltaTime);
        rollAngleCurrent = Mathf.SmoothDamp(rollAngleCurrent, 0f, ref rollVelocity, 0.4f);

        // Keep heading locked: NO circles
        yawAngle = lockedDiveYaw;

        currentSpeed = Mathf.MoveTowards(currentSpeed, outOfEnergyForwardSpeed, 20f * Time.deltaTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(pitchAngle, yawAngle, rollAngleCurrent);

        if (followPlaneWhileDiving)
            EnableAndPlaceCrashTerrain();

        if (transform.position.y <= crashStopY)
        {
            transform.position = new Vector3(transform.position.x, crashStopY, transform.position.z);

            currentSpeed = 0f;
            energyDraining = false;
            if (runUI) runUI.EndRun();
            enabled = false;
        }
    }

    void EnableAndPlaceCrashTerrain()
    {
        if (crashTerrain == null || crashTerrain.terrainData == null) return;

        if (!crashTerrain.gameObject.activeSelf)
            crashTerrain.gameObject.SetActive(true);

        Vector3 size = crashTerrain.terrainData.size;

        // We want the plane roughly centered on the terrain block.
        Vector3 desiredCenter = transform.position + crashTerrainOffset;

        // Terrain position is its bottom-left corner, so subtract half size.
        crashTerrain.transform.position = new Vector3(
            desiredCenter.x - size.x * 0.5f,
            crashTerrainY,
            desiredCenter.z - size.z * 0.5f
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ring"))
        {
            energy += rechargeAmount;
            energy = Mathf.Clamp(energy, 0f, maxEnergy);
        }
    }
}
