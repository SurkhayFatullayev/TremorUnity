using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the Player has the "Player" tag
        {
            Debug.Log("⚡ Enemy hit the Player!");
            
            // (Optional) Reduce player's health (if health system exists)
            // other.GetComponent<PlayerHealth>()?.TakeDamage(10);

            // Destroy the projectile on impact
            Destroy(gameObject);
        }
    }
}
