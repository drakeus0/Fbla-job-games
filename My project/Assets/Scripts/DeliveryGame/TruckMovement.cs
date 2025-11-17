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
    public float acceleration = 3000f;
    public float brakeForce = 8000f;
    public float maxTurnAngle = 40f;
    public float maxSpeed = 55f;
    public float reverseSpeedMultiplier = 1f;
    public float turnLeanAngle = 10f;

    private float currentAcceleration = 0f;
    private float currentTurnAngle = 0f;

    bool reversing = false;

    private void Start()
    {
        if (carRB == null) carRB = GetComponent<Rigidbody>();
        carRB.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    private void FixedUpdate()
    {
        // --- Input ---
        float vertical = 0f;
        float horizontal = 0f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            vertical += 1f; // W = forward
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            vertical -= 1f; // S = brake/reverse

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            horizontal += 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            horizontal -= 1f;

        Vector2 input = new Vector2(horizontal, vertical);
        if (input.magnitude > 1f) input.Normalize();

        // --- Steering ---
        currentTurnAngle = maxTurnAngle * input.x;
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;

        // --- Motor & Braking ---
        Vector3 flatVelocity = new Vector3(carRB.linearVelocity.x, 0, carRB.linearVelocity.z);

        // Determine if car is essentially stopped
        Debug.Log(reversing);
        Debug.Log(flatVelocity.magnitude);
        // --- Acceleration logic ---
        if (Keyboard.current.sKey.isPressed && flatVelocity.magnitude < 2f)
        {
            // Start reversing
            reversing = true;
            Debug.Log("thjios ran");
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            // Brake while moving forward
            currentAcceleration = 0f;
            ApplyBrakes(brakeForce);
        }
        else if (Keyboard.current.wKey.isPressed)
        {
            // Accelerate forward normally
            currentAcceleration = acceleration;
            ApplyBrakes(0f);
           reversing = false;
        }
        else
        {
            currentAcceleration = 0f;
            ApplyBrakes(0f);
            reversing = false;
        }

        if (reversing)
        {
            currentAcceleration = -acceleration * reverseSpeedMultiplier;
            ApplyBrakes(0f);
        }

        // Apply torque
        backLeft.motorTorque = -currentAcceleration;
        backRight.motorTorque = -currentAcceleration;

        // --- Limit speed ---
        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 limited = flatVelocity.normalized * maxSpeed;
            carRB.linearVelocity = new Vector3(limited.x, carRB.linearVelocity.y, limited.z);
        }

        // --- Update wheels ---
        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
        UpdateWheel(backRight, backRightTransform);

        // --- Visual lean ---
        if (flatVelocity.magnitude < 5f)
        {
        float targetLean = -input.x * turnLeanAngle;
        Quaternion leanRotation = Quaternion.Euler(0f, 0f, targetLean);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0f) * leanRotation,
            Time.deltaTime * 4f
        );
        }
    }

    private void ApplyBrakes(float brake)
    {
        frontLeft.brakeTorque = brake;
        frontRight.brakeTorque = brake;
        backLeft.brakeTorque = brake;
        backRight.brakeTorque = brake;
    }

    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);

        trans.position = Vector3.Lerp(trans.position, pos, Time.deltaTime * 10f);
        trans.rotation = Quaternion.Slerp(trans.rotation, rot, Time.deltaTime * 10f);

        float rotationAngle = col.rpm * 6f * Time.deltaTime;
        trans.Rotate(Vector3.right, rotationAngle, Space.Self);
    }
}
