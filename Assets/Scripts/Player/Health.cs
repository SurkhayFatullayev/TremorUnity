using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f; // Object's health

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Remaining Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject); // Destroy the enemy
    }
}
