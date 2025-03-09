using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Enemy took damage, current health: " + health);
        if (health <= 0)
        {
            Die();
        }
    }

    public void ActivateRagdoll()
    {
        // If you have a ragdoll system, activate it here.
        Debug.Log("Ragdoll activated!");
        // Example: GetComponent<Rigidbody>().isKinematic = false;
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);  // Destroy enemy after death
    }
}
