using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public event System.Action<int> onDoorwayTriggerEnter;
    public void DoorwayTriggerEnter(int id)
    {
        onDoorwayTriggerEnter?.Invoke(id);
    }

    public event System.Action<int> onDoorwayTriggerExit;
    public void DoorwayTriggerExit(int id)
    {
        onDoorwayTriggerExit?.Invoke(id);
    }
}
