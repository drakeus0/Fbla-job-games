using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SimpleCarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody carRB;
    [Tooltip("Visual root of the car (mesh). This is what will lean/tilt. Do NOT rotate the physics root to fix model orientation; rotate this mesh if needed).")]
    [SerializeField] private Transform carVisual;

    [Header("Wheel Colliders (assign)")]
    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] private WheelCollider backRight;
    [SerializeField] private WheelCollider backLeft;

    [Header("Wheel Transforms (visual meshes)")]
    [SerializeField] private Transform frontRightTransform;
    [SerializeField] private Transform frontLeftTransform;
    [SerializeField] private Transform backRightTransform;
    [SerializeField] private Transform backLeftTransform;

    [Header("Settings")]
    public float acceleration = 3000f;        
    public float brakeForce = 8000f;        
    public float maxTurnAngle = 40f;          
    public float maxSpeed = 55f;            
    public float reverseSpeedMultiplier = 0.5f; 
    public float turnLeanAngle = 6f;         
    public float leanSpeedForFullEffect = 20f;  

    private float currentAcceleration = 0f;    
    private float currentTurnAngle = 0f;
    private bool reversing = false;

    private void Reset()
    {
        carRB = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (carRB == null) carRB = GetComponent<Rigidbody>();
        carRB.centerOfMass = new Vector3(0f, -1f, 0f);
    }

    private void FixedUpdate()
    {
        float vertical = 0f;
        float horizontal = 0f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            vertical += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            vertical -= 1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            horizontal += 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            horizontal -= 1f;

        Vector2 input = new Vector2(horizontal, vertical);
        if (input.magnitude > 1f) input.Normalize();

        float targetSteer = maxTurnAngle * input.x;
        currentTurnAngle = Mathf.Lerp(currentTurnAngle, targetSteer, Time.fixedDeltaTime * 6f);
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;

        if (carRB.linearVelocity.magnitude > 0.1f)
        {
            float steerTorque = currentTurnAngle * 0.1f; // tweak 0.1f to feel right
            carRB.AddTorque(Vector3.up * steerTorque, ForceMode.Acceleration);
        }

        Vector3 lv = carRB.linearVelocity;

        // Only smooth upward motion, let gravity handle falling naturally
        if (lv.y > 0f)
        {
            lv.y = Mathf.Lerp(lv.y, 0f, Time.fixedDeltaTime * 5f);
        }

        carRB.linearVelocity = lv;

        // Calculate flat speed for acceleration/braking
        Vector3 flatVelocity = new Vector3(lv.x, 0f, lv.z);
        float speed = flatVelocity.magnitude;


        bool wDown = Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed;
        bool sDown = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;

        currentAcceleration = 0f;
        ApplyBrakes(0f);

        if (sDown)
        {
            if (!wDown && speed < 1f)
            {
                reversing = true;
            }

            if (!reversing && speed > 0.1f)
            {
                ApplyBrakes(brakeForce);
                currentAcceleration = 0f;
            }
        }
        else if (wDown)
        {
            reversing = false;
            currentAcceleration = acceleration;
            ApplyBrakes(0f);
        }
        else
        {
            currentAcceleration = 0f;
            ApplyBrakes(0f);
            reversing = false;
        }

        if (reversing)
        {
            ApplyBrakes(0f);
            currentAcceleration = -acceleration * reverseSpeedMultiplier;
        }

        backLeft.motorTorque = currentAcceleration;
        backRight.motorTorque = currentAcceleration;

        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 limited = flatVelocity.normalized * maxSpeed;
            carRB.linearVelocity = new Vector3(limited.x, carRB.linearVelocity.y, limited.z);
        }

        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
        UpdateWheel(backRight, backRightTransform);

        if (carVisual != null)
        {
            float speedFactor = Mathf.Clamp01(speed / leanSpeedForFullEffect);
            float targetZ = -input.x * turnLeanAngle * speedFactor; 
            Quaternion targetLocal = Quaternion.Euler(0f, 0f, targetZ);

            Vector3 currentLocalEuler = carVisual.localEulerAngles;
            float currentZ = NormalizeAngle(currentLocalEuler.z);
            float desiredZ = NormalizeAngle(targetZ);
            float newZ = Mathf.LerpAngle(currentZ, desiredZ, Time.fixedDeltaTime * 4f);

            carVisual.localRotation = Quaternion.Euler(currentLocalEuler.x, currentLocalEuler.y, newZ);
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
        if (col == null || trans == null) return;

        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        trans.position = pos;
        trans.rotation = rot;
    }

    private static float NormalizeAngle(float a)
    {
        a %= 360f;
        if (a > 180f) a -= 360f;
        if (a < -180f) a += 360f;
        return a;
    }
}
