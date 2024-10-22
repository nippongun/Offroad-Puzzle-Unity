using UnityEngine;
using Interactable;
using Damageable;
public class TestCube : MonoBehaviour, IInteractable, IDamageable
{
    [SerializeField] private int health = 100;
    public int Health { get => health; set => health = value; }
    public void Interact()
    {
        Debug.Log("Interacted");
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log("I took " + damage + " damage");
        if (Health <= 0)
        {
            Debug.Log("I died");
            Health = 100;
        }
    }
}
