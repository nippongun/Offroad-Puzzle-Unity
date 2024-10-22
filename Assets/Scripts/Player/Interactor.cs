using UnityEngine;
using UnityEngine.InputSystem;
namespace Interactable
{
    interface IInteractable
    {
        public void Interact();
    }

    public class Interactor : MonoBehaviour
    {
        private Transform InteractorSource;
        public float InteractorRange = 5f;
        public InputActionReference interactAction = null;

        private void Awake()
        {
            // Ensure the action is not null
            if (interactAction == null)
            {
                Debug.LogError("Interact action is not assigned in the Inspector!");
                return;
            }

            // Set up the action callback
            interactAction.action.performed += HandleInteract;
            InteractorSource = transform;
        }

        private void OnEnable()
        {
            interactAction.action.Enable();
        }

        private void OnDisable()
        {
            interactAction.action.Disable();
        }

        private void OnDestroy()
        {
            // Clean up the callback when the object is destroyed
            if (interactAction != null)
            {
                interactAction.action.performed -= HandleInteract;
            }
        }

        private void HandleInteract(InputAction.CallbackContext context)
        {
            Debug.Log("Interacting...");

            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, InteractorRange))
            {
                Debug.Log("Raycast hit: " + raycastHit.transform.name);
                if (raycastHit.transform.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    Debug.Log("Interactable found: " + interactable);
                    interactable.Interact();
                }
            }
        }
    }
}
