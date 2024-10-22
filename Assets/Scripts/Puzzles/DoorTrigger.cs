using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public int id;
    [SerializeField] bool forCar = false;
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            GameEvents.current.DoorwayTriggerEnter(id);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            GameEvents.current.DoorwayTriggerExit(id);
        }
    }
}
