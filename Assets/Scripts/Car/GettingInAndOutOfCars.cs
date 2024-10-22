using TMPro;
using UnityEngine;
using Unity.Cinemachine;

public class GettingInAndOutOfCars : MonoBehaviour
{
    [SerializeField] GameObject player = null;
    [SerializeField] GameObject car = null;
    [SerializeField] KeyCode enterExitKey = KeyCode.F;

    [SerializeField] PrometeoCarController carController = null;
    public CinemachineCamera playerCamera;
    public CinemachineCamera carCamera;
    void Start()
    {
        SwitchToCarCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(enterExitKey))
        {
            GetOutOfCar();
        }
    }

    void GetOutOfCar()
    {
        player.SetActive(true);

        player.transform.position = car.transform.position + car.transform.TransformDirection(Vector3.left);

        carController.enabled = false;

        SwitchToPlayerCamera();
    }

    private void SwitchToCarCamera()
    {
        playerCamera.gameObject.SetActive(false);
        carCamera.gameObject.SetActive(true);
    }

    private void SwitchToPlayerCamera()
    {
        playerCamera.gameObject.SetActive(true);
        carCamera.gameObject.SetActive(false);
    }
}
