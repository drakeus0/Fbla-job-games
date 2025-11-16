using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SimpleCarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody carRB;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] private WheelCollider backRight;
    [SerializeField] private WheelCollider backLeft;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontRightTransform;
    [SerializeField] private Transform frontLeftTransform;
    [SerializeField] private Transform backRightTransform;
    [SerializeField] private Transform backLeftTransform;

    [Header("Settings")]
    public float acceleration = 5000f;       // Cartoonish acceleration
    public float brakeForce = 8000f;         // Strong braking
    public float maxTurnAngle = 40f;         // Exaggerated turning
    public float maxSpeed = 55f;             // m/s (~200 km/h)
    public float reverseSpeedMultiplier = 0.5f; // half speed in reverse
    public float turnLeanAngle = 10f;        // Visual tilt when turning

    private float currentAcceleration = 0f;
    private float currentTurnAngle = 0f;

    private void Start()
    {
        if (carRB == null)
            carRB = GetComponent<Rigidbody>();

        // Lower center of mass for stability
        carRB.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    private void FixedUpdate()
    {
        // --- Input ---
        float vertical = 0f;
        float horizontal = 0f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            vertical -= 1f; // W = forward
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            vertical += 1f; // S = backward
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            horizontal += 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            horizontal -= 1f;

        // Normalize diagonal input
        Vector2 input = new Vector2(horizontal, vertical);
        if (input.magnitude > 1f)
            input.Normalize();

        // --- Steering ---
        currentTurnAngle = maxTurnAngle * input.x;
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;

        // --- Motor torque ---
        currentAcceleration = input.y * acceleration;

        // Automatic reverse if holding S after stop
        Vector3 horizontalVelocity = new Vector3(carRB.linearVelocity.x, 0f, carRB.linearVelocity.z);
        if (Keyboard.current.sKey.isPressed && horizontalVelocity.magnitude < 0.1f)
        {
            currentAcceleration = -acceleration * reverseSpeedMultiplier;
        }

        frontLeft.motorTorque = currentAcceleration;
        frontRight.motorTorque = currentAcceleration;

        // --- Limit speed ---
        Vector3 flatVelocity = new Vector3(carRB.linearVelocity.x, 0f, carRB.linearVelocity.z);
        if (flatVelocity.magnitude > maxSpeed)
        {
            flatVelocity = flatVelocity.normalized * maxSpeed;
            carRB.linearVelocity = new Vector3(flatVelocity.x, carRB.linearVelocity.y, flatVelocity.z);
        }

        // --- Update wheel visuals ---
        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
        UpdateWheel(backRight, backRightTransform);

        // --- Visual lean for cartoon effect ---
        float targetLean = -input.x * turnLeanAngle; // tilt opposite direction of turn
        Quaternion leanRotation = Quaternion.Euler(0f, 0f, targetLean);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0f) * leanRotation, Time.deltaTime * 3f);
    }

    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);

        // Smooth position
        trans.position = Vector3.Lerp(trans.position, pos, Time.deltaTime * 10f);

        // Smooth rotation
        trans.rotation = Quaternion.Slerp(trans.rotation, rot, Time.deltaTime * 10f);

        // Rotate around axle based on wheel RPM
        float rotationAngle = col.rpm * 6f * Time.deltaTime;
        trans.Rotate(Vector3.right, rotationAngle, Space.Self);
    }
}
