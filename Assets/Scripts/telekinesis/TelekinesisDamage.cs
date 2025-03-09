using UnityEngine;

public class TelekinesisDamage : MonoBehaviour
{
    private float damage = 50f; // Fixed damage to 50


    public void SetDamage(float chargeTime)
{
    damage = Mathf.Clamp(chargeTime * 10f, 5f, 50f); // Ensure damage is within range
    Debug.Log($"🔵 Damage Set to: {damage}"); // ✅ Debugging
}


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) // Ensure enemies have the "Enemy" tag
        {
            Debug.Log($"⚡ TeleObject {gameObject.name} hit enemy: {collision.gameObject.name}! Applying 50 damage.");

            // (Optional) If enemy has a health system, apply damage
            Health currentHealth = collision.gameObject.GetComponent<Health>();
            if (currentHealth != null)
            {
                Debug.Log($"🟠 Before Damage: {currentHealth.health} HP"); // Debug before applying damage
                currentHealth.TakeDamage(Mathf.RoundToInt(damage)); 
                Debug.Log($"🔴 After Damage: {currentHealth.health} HP"); // Debug after applying damage
            }

            // Destroy object after impact (Optional)
            Destroy(gameObject, 1f);
        }
    }
}
