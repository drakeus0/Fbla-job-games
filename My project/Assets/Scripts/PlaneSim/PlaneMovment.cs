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

    [Header("UI Prompts")]
    [SerializeField] GameObject flightControlsPrompt;
    [SerializeField] float flightPromptDuration = 5f;
    bool flightPromptShown = false;

    [Header("Airspeed Gauge")]
    [SerializeField] private UIAirspeedGauge airspeedGauge;

    [Tooltip("What the gauge should read when the plane is at maxFlySpeed (ex: 200).")]
    [SerializeField] private float gaugeMaxKnots = 200f;

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
    public float outOfEnergyDivePitch = 35f;        // try 25–40 for a “dive forward” feel
    public float outOfEnergyForwardSpeed = 35f;     // bump this up if you want more glide
    public float crashStopY = 0f;

    [Tooltip("Downward fall speed (scaled by currentSpeed). 0.3 = shallow glide, 1.0 = steep fall.")]
    public float diveDownMultiplier = 0.6f;

    bool outOfEnergy = false;
    float lockedDiveYaw;

    [Header("Crash Ground Plane (instead of Terrain)")]
    public GameObject crashGroundPlane;     // drag a big Plane object here
    public float crashGroundY = 0f;         // usually 0
    public Vector3 crashGroundOffset;       // e.g. (0,0,200) if you want it ahead
    public bool followPlaneWhileDiving = true;

    [Header("Disable plane collider during dive (prevents spin-outs)")]
    public bool disablePlaneColliderOnDive = true;

    [Header("Takeoff Gate (Tutorial UI)")]
    [SerializeField] bool requireTutorialComplete = true;

    [Tooltip("Optional: parent object that contains the whole tutorial UI. If it gets disabled, takeoff is allowed.")]
    [SerializeField] GameObject tutorialUIRoot;

    bool tutorialComplete = false;

    public RunStatsUI runUI;

    Collider planeCollider;

    public enum FlightState { Grounded, TakingOff, Flying }
    public FlightState flightState = FlightState.Grounded;

    IEnumerator ShowFlightControlsPrompt()
    {
        if (flightControlsPrompt == null) yield break;

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

        if (crashGroundPlane != null)
            crashGroundPlane.SetActive(false);

        // Initialize gauge at 0
        UpdateAirspeedGauge(0f);
    }

    void Update()
    {
        if (!tutorialComplete && tutorialUIRoot != null && !tutorialUIRoot.activeInHierarchy)
            tutorialComplete = true;

        bool takeoffHeld =
            (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            && CanTakeOff();

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

        // Keep gauge at 0 on the ground
        UpdateAirspeedGauge(currentSpeed);

        if (takeoffHeld)
        {
            flightState = FlightState.TakingOff;

            if (takeoffText != null)
                takeoffText.SetActive(false);
        }
    }

    bool CanTakeOff()
    {
        if (!requireTutorialComplete) return true;
        return tutorialComplete;
    }

    public void MarkTutorialComplete()
    {
        tutorialComplete = true;

        if (tutorialUIRoot != null)
            tutorialUIRoot.SetActive(false);
    }

    void HandleTakeoff()
    {
        currentSpeed += 45f * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, takeoffSpeed);

        pitchAngle = Mathf.MoveTowards(pitchAngle, -takeoffPitchAngle, 60f * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(pitchAngle, yawAngle, 0f);

        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        // Update gauge during takeoff
        UpdateAirspeedGauge(currentSpeed);

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

        // Update gauge while flying
        UpdateAirspeedGauge(currentSpeed);

        pitchVelocity += verticalInput * pitchSpeed * Time.deltaTime;
        pitchVelocity = Mathf.Lerp(pitchVelocity, 0f, 3f * Time.deltaTime);
        pitchAngle += pitchVelocity * Time.deltaTime;
        pitchAngle = Mathf.Clamp(pitchAngle, -maxPitchAngle, maxPitchAngle);

        float targetRoll = -horizontalInput * rollAngle;
        rollAngleCurrent = Mathf.SmoothDamp(rollAngleCurrent, targetRoll, ref rollVelocity, 0.6f);

        yawAngle += -rollAngleCurrent * 0.4f * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(pitchAngle, yawAngle, rollAngleCurrent);

        if (energy <= 0f)
        {
            outOfEnergy = true;
            lockedDiveYaw = yawAngle;

            pitchVelocity = 0f;
            rollVelocity = 0f;

            if (disablePlaneColliderOnDive && planeCollider != null)
                planeCollider.enabled = false;

            EnableAndPlaceCrashGround();
        }
    }

    void HandleOutOfEnergyDive()
    {
        // Rotate toward the dive pose
        pitchAngle = Mathf.MoveTowards(pitchAngle, outOfEnergyDivePitch, 80f * Time.deltaTime);
        rollAngleCurrent = Mathf.SmoothDamp(rollAngleCurrent, 0f, ref rollVelocity, 0.4f);

        // Lock yaw so it doesn't keep steering
        yawAngle = lockedDiveYaw;

        // Apply rotation FIRST (so visuals match)
        transform.localRotation = Quaternion.Euler(pitchAngle, yawAngle, rollAngleCurrent);

        // Ease toward a consistent forward speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, outOfEnergyForwardSpeed, 20f * Time.deltaTime);

        // Update gauge during dive
        UpdateAirspeedGauge(currentSpeed);

        // ✅ MOVE: forward direction from yaw only (always glides forward)
        Vector3 yawForward = Quaternion.Euler(0f, yawAngle, 0f) * Vector3.forward;

        transform.position += yawForward * currentSpeed * Time.deltaTime;
        transform.position += Vector3.down * (currentSpeed * diveDownMultiplier) * Time.deltaTime;

        if (followPlaneWhileDiving)
            EnableAndPlaceCrashGround();

        if (transform.position.y <= crashStopY)
        {
            transform.position = new Vector3(transform.position.x, crashStopY, transform.position.z);

            currentSpeed = 0f;
            energyDraining = false;

            // Snap gauge to 0 on crash
            UpdateAirspeedGauge(0f);

            if (runUI) runUI.EndRun();
            enabled = false;
        }
    }

    void EnableAndPlaceCrashGround()
    {
        if (crashGroundPlane == null) return;

        if (!crashGroundPlane.activeSelf)
            crashGroundPlane.SetActive(true);

        Vector3 desiredCenter = transform.position + crashGroundOffset;

        crashGroundPlane.transform.position = new Vector3(
            desiredCenter.x,
            crashGroundY,
            desiredCenter.z
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

    // ---------- Gauge Helpers ----------
    void UpdateAirspeedGauge(float speedUnitsPerSecond)
    {
        if (airspeedGauge == null) return;

        // Map your in-game speed (minFlySpeed..maxFlySpeed) into the gauge range (0..gaugeMaxKnots).
        float t = Mathf.InverseLerp(minFlySpeed, maxFlySpeed, speedUnitsPerSecond);
        float knots = Mathf.Lerp(0f, gaugeMaxKnots, t);

        airspeedGauge.SetSpeedKnots(knots);
    }
}
