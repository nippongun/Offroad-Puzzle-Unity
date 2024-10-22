using UnityEngine;
namespace Damageable
{
    public class TestDamage : MonoBehaviour, IDamageable
    {
        public int Health { get; set; } = 100;
        public void TakeDamage(int damage)
        {
            Health -= damage;
            Debug.Log("I took " + damage + " damage");
        }

    }
};
