using UnityEngine;

public class ShipMotor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Linear Movement")]
    [SerializeField] private float thrustPower = 20f;
    [SerializeField] private float strafePower = 12f;
    [SerializeField] private float liftPower = 12f;
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float linearDrag = 1.5f;

    [Header("Angular Movement")]
    [SerializeField] private float pitchTorque = 8f;
    [SerializeField] private float yawTorque = 8f;
    [SerializeField] private float rollTorque = 10f;
    [SerializeField] private float maxAngularSpeed = 2.5f;
    [SerializeField] private float angularDrag = 3f;

    [Header("Assist")]
    [SerializeField] private float lateralDamping = 2f;
    [SerializeField] private float autoLevelStrength = 1f;

    private float thrustInput;
    private float strafeInput;
    private float liftInput;
    private float pitchInput;
    private float yawInput;
    private float rollInput;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.drag = linearDrag;
        rb.angularDrag = angularDrag;
    }

    public void SetInput(
        float thrust,
        float strafe,
        float lift,
        float pitch,
        float yaw,
        float roll)
    {
        thrustInput = Mathf.Clamp(thrust, -1f, 1f);
        strafeInput = Mathf.Clamp(strafe, -1f, 1f);
        liftInput = Mathf.Clamp(lift, -1f, 1f);
        pitchInput = Mathf.Clamp(pitch, -1f, 1f);
        yawInput = Mathf.Clamp(yaw, -1f, 1f);
        rollInput = Mathf.Clamp(roll, -1f, 1f);
    }

    private void FixedUpdate()
    {
        ApplyTranslation();
        ApplyRotation();
        ApplyAssist();
        ClampVelocities();
    }

    private void ApplyTranslation()
    {
        Vector3 force =
            transform.forward * thrustInput * thrustPower +
            transform.right * strafeInput * strafePower +
            transform.up * liftInput * liftPower;

        rb.AddForce(force, ForceMode.Acceleration);
    }

    private void ApplyRotation()
    {
        Vector3 torque =
            transform.right * pitchInput * pitchTorque +
            transform.up * yawInput * yawTorque +
            -transform.forward * rollInput * rollTorque;

        rb.AddTorque(torque, ForceMode.Acceleration);

        if (Mathf.Abs(rollInput) < 0.01f)
        {
            Vector3 localUp = transform.up;
            float rollError = Vector3.Dot(localUp, Vector3.right);
            rb.AddTorque(-transform.forward * rollError * autoLevelStrength, ForceMode.Acceleration);
        }
    }

    private void ApplyAssist()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);

        localVelocity.x = Mathf.Lerp(localVelocity.x, 0f, lateralDamping * Time.fixedDeltaTime);
        localVelocity.y = Mathf.Lerp(localVelocity.y, 0f, lateralDamping * Time.fixedDeltaTime);

        rb.velocity = transform.TransformDirection(localVelocity);
    }

    private void ClampVelocities()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        if (rb.angularVelocity.magnitude > maxAngularSpeed)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularSpeed;
        }
    }
}