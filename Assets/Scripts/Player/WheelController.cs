using UnityEngine;

public class WheelController : MonoBehaviour
{
    public Transform wheelModel;
    [HideInInspector]
    public WheelCollider wheelCollider;
    public bool steerable;
    public bool motorized;

    Vector3 position;
    Quaternion rotation;

    private void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    void Update()
    {
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelCollider.transform.position = position;
        wheelModel.transform.rotation = rotation;
    }
}
