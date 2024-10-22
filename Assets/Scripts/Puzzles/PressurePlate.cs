using System;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Material triggered;
    public Material notTriggered;
    public int id;
    [SerializeField] string desiredTag = "Player";
    private bool isTriggered = false;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Entered: {other.gameObject.tag}");
        if (other.gameObject.tag == desiredTag && !isTriggered)
        {
            isTriggered = true;
            GameEvents.current.DoorwayTriggerEnter(id);
            SwapMaterial();
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"Exited: {other.gameObject.tag}");
        if (other.gameObject.tag == desiredTag && isTriggered)
        {
            isTriggered = false;
            GameEvents.current.DoorwayTriggerExit(id);
            SwapMaterial();
        }
    }

    void SwapMaterial()
    {
        GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material == triggered ? notTriggered : triggered;
    }
}
