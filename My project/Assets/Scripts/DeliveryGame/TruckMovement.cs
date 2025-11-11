using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SimpleCarController : MonoBehaviour
{
    [SerializeField] Rigidbody CarRB;

    [SerializeField] WheelCollider FrontRight;
    [SerializeField] WheelCollider FrontLeft;
    [SerializeField] WheelCollider BackRight;
    [SerializeField] WheelCollider BackLeft;

    public float acceleration = 500f;
    public float breakingForce = 300f;

    float currentAcceleration = 0f;
    float currentBreakForce = 0f;
    private void Start()
    {
        CarRB.centerOfMass = new Vector3(0, -0.5f, 0);
    }
    private void FixedUpdate()
    {

        float vertical = 0f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            vertical += 1f;

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            vertical -= 1f;

        currentAcceleration = vertical * acceleration;

        if (Keyboard.current.spaceKey.isPressed)
        {
            currentBreakForce = breakingForce;
        }
        else
        {
            currentBreakForce = 0f;
        }
        FrontRight.motorTorque = currentAcceleration;
        FrontLeft.motorTorque = currentAcceleration;

        FrontRight.brakeTorque = currentBreakForce;
        FrontLeft.brakeTorque = currentBreakForce;
        BackRight.brakeTorque = currentBreakForce;
        BackLeft.brakeTorque = currentBreakForce;
    }
}
