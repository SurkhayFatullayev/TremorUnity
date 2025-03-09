using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f; // Object's health
    public HealthBar healthBar; // Reference to HealthBar

    void Start()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHealth((int)health); // Ensure healthBar is set to max
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Remaining Health: " + health);

        if (healthBar != null)
        {
            healthBar.SetHealth((int)health); // Update health bar
        }

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
