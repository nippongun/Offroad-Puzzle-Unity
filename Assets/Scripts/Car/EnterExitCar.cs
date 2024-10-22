using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera carCamera = null;
    private CinemachineCamera playerCamera = null;
    private GameObject player = null;
    [SerializeField] PrometeoCarController carController = null;
    public InputActionReference interactAction = null;

    private bool isPlayerInsideCar = false;
    private bool isPlayerNearby = false;

    private void OnEnable()
    {
        interactAction.action.performed += OnInteract;
        interactAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }
    private void EnterCar()
    {
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
        }
        carCamera.gameObject.SetActive(true);
        isPlayerInsideCar = true;
        player.SetActive(false);
        carController.enabled = true;
    }

    private void ExitCar()
    {
        try
        {
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(true);
            }
            carCamera.gameObject.SetActive(false);
            carController.enabled = false;

            isPlayerInsideCar = false;
            player.transform.position = transform.position + transform.TransformDirection(Vector3.left);
            player.SetActive(true);
            player = null;
        }
        catch
        {
            Debug.LogAssertion("Player not found");
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isPlayerNearby)
        {
            if (isPlayerInsideCar)
            {
                ExitCar();
            }
            else
            {
                EnterCar();
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            // Try to find the player's camera
            playerCamera = other.GetComponentInChildren<CinemachineCamera>();
            if (playerCamera == null)
            {
                Debug.LogWarning("Player camera not found. Make sure the player has a CinemachineCamera component.");
            }

            Debug.Log("Player entered car");
            player = other.gameObject;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Player left car");
        }
    }
}