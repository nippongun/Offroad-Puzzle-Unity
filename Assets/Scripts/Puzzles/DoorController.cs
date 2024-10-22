using UnityEngine;

public class DoorController : MonoBehaviour
{
    public int id;
    public int requiredTriggers = 2;
    private int activeTriggers = 0;
    private bool isOpen = false;

    void Start()
    {
        GameEvents.current.onDoorwayTriggerEnter += OnDoorwayTriggerEnter;
        GameEvents.current.onDoorwayTriggerExit += OnDoorwayTriggerExit;
    }

    private void OnDoorwayTriggerEnter(int id)
    {
        if (id == this.id)
        {
            activeTriggers++;
            if (activeTriggers >= requiredTriggers && !isOpen)
            {
                OpenDoor();
            }
        }
    }

    private void OnDoorwayTriggerExit(int id)
    {
        if (id == this.id)
        {
            activeTriggers--;
            if (activeTriggers < requiredTriggers && isOpen)
            {
                CloseDoor();
            }
        }
    }

    private void OpenDoor()
    {
        transform.position += Vector3.up * 2;
        isOpen = true;
    }

    private void CloseDoor()
    {
        transform.position -= Vector3.up * 2;
        isOpen = false;
    }

    private void OnDestroy()
    {
        GameEvents.current.onDoorwayTriggerEnter -= OnDoorwayTriggerEnter;
        GameEvents.current.onDoorwayTriggerExit -= OnDoorwayTriggerExit;
    }
}
