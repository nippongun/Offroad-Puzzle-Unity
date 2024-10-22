using UnityEngine;

public class WheelBasedCarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody carRigidBody;
    [SerializeField] private Transform[] tireTransforms;
    [SerializeField] private Transform[] steeringTires;

    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float springTravel;
    [SerializeField] private float restLength;
    [SerializeField] private float wheelRadius;

    [Header("Acceleration Settings")]
    [SerializeField] private float acceleration;
    [SerializeField] private float maxForwardSpeed = 120f;
    [SerializeField] private float maxReverseSpeed = 30f;
    [SerializeField] private AnimationCurve powerCurve;
    [SerializeField] private bool fourWheelDrive = true;
    private float accelerationInput;

    [Header("Steer Settings")]
    [SerializeField] private float tireGripFactor;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float tireMass;
    private float steeringInput;

    [Header("Braking Settings")]
    [SerializeField] private float brakeTorque = 2000f;
    [SerializeField] private float rollingResistance = 0.05f;

    private float currentAngle = 0f;

    private void Start()
    {
        carRigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetInput();
        RotateSteeringTires();
    }

    private void FixedUpdate()
    {
        ApplyWheelForces();
        ApplyRollingResistance();
    }

    private void GetInput()
    {
        steeringInput = Input.GetAxis("Horizontal");
        accelerationInput = Input.GetAxis("Vertical");
    }

    private void RotateSteeringTires()
    {
        float targetAngle = steeringInput * maxSteerAngle;
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, 10f * Time.deltaTime);

        Quaternion steerRotation = Quaternion.Euler(0f, currentAngle, 0f);

        foreach (Transform wheel in steeringTires)
        {
            wheel.localRotation = steerRotation;
        }
    }

    private void ApplyWheelForces()
    {
        foreach (Transform tire in tireTransforms)
        {
            if (TryGetWheelHitInfo(tire, out WheelHitInfo hitInfo))
            {
                ApplySuspensionForce(hitInfo);
                ApplySteeringForce(hitInfo);
                ApplyAccelerationForce(hitInfo);
                if (accelerationInput == 0f && carRigidBody.linearVelocity.z < 0.1f)
                {
                    ApplyBrakingForce(hitInfo);
                }
            }
        }
    }

    private bool TryGetWheelHitInfo(Transform tire, out WheelHitInfo hitInfo)
    {
        hitInfo = new WheelHitInfo();
        float maxLength = restLength + springTravel;

        if (Physics.Raycast(tire.position, -tire.up, out RaycastHit hit, maxLength + wheelRadius))
        {
            hitInfo.Hit = hit;
            hitInfo.SpringDir = tire.up;
            hitInfo.TireWorldVelocity = carRigidBody.GetPointVelocity(tire.position);
            hitInfo.Offset = restLength - hit.distance;
            hitInfo.TireTransform = tire;

            Debug.DrawLine(tire.position, hit.point, Color.green);
            return true;
        }

        Debug.DrawLine(tire.position, tire.position + (wheelRadius + maxLength) * -tire.up, Color.yellow);
        return false;
    }

    private void ApplySuspensionForce(WheelHitInfo hitInfo)
    {
        float velocity = Vector3.Dot(hitInfo.SpringDir, hitInfo.TireWorldVelocity);
        float force = (hitInfo.Offset * springStiffness) - (velocity * damperStiffness);
        carRigidBody.AddForceAtPosition(hitInfo.SpringDir * force, hitInfo.TireTransform.position);
    }

    private void ApplySteeringForce(WheelHitInfo hitInfo)
    {
        Vector3 steeringDir = hitInfo.TireTransform.right;
        float steeringVelocity = Vector3.Dot(steeringDir, hitInfo.TireWorldVelocity);
        float desiredVelocity = -steeringVelocity * tireGripFactor;
        float desiredAcceleration = desiredVelocity / Time.fixedDeltaTime;

        carRigidBody.AddForceAtPosition(steeringDir * desiredAcceleration * tireMass, hitInfo.TireTransform.position);
        Debug.DrawLine(hitInfo.TireTransform.position, hitInfo.TireTransform.position + steeringDir, Color.red);
    }

    private void ApplyAccelerationForce(WheelHitInfo hitInfo)
    {
        Vector3 accelerationDir = hitInfo.TireTransform.forward;
        float carSpeed = Vector3.Dot(transform.forward, carRigidBody.linearVelocity);
        float maxSpeed = accelerationInput >= 0 ? maxForwardSpeed : maxReverseSpeed;
        float normalizedCarSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / maxSpeed);

        Debug.Log($"Car speed: {carSpeed}, normalized: {normalizedCarSpeed}");

        if (Mathf.Abs(accelerationInput) > 0.1f)
        {
            float availableTorque = powerCurve.Evaluate(normalizedCarSpeed) * Mathf.Sign(accelerationInput);
            carRigidBody.AddForceAtPosition(accelerationDir * availableTorque * acceleration, hitInfo.TireTransform.position);
            Debug.DrawLine(hitInfo.TireTransform.position, hitInfo.TireTransform.position + (accelerationDir * availableTorque), Color.blue);
        }
        else if (false)
        {
            ApplyBrakingForce(hitInfo);
            Debug.DrawLine(hitInfo.TireTransform.position, hitInfo.TireTransform.position + (-hitInfo.TireWorldVelocity.normalized * brakeTorque), Color.magenta);
        }
    }

    private void ApplyBrakingForce(WheelHitInfo hitInfo)
    {
        Vector3 brakeForce = -hitInfo.TireWorldVelocity.normalized * brakeTorque;
        carRigidBody.AddForceAtPosition(brakeForce, hitInfo.TireTransform.position);
    }

    private void ApplyRollingResistance()
    {
        Vector3 rollingResistanceForce = -carRigidBody.linearVelocity * rollingResistance;
        carRigidBody.AddForce(rollingResistanceForce, ForceMode.Acceleration);
    }

    private struct WheelHitInfo
    {
        public RaycastHit Hit;
        public Vector3 SpringDir;
        public Vector3 TireWorldVelocity;
        public float Offset;
        public Transform TireTransform;
    }
}