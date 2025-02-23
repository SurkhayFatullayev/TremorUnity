using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TelekinesisDamage : MonoBehaviour
{
    private float damageAmount = 0f;

    public void SetDamage(float chargeTime)
    {
        // Calculate damage based on charge time (you can customize this formula)
        damageAmount = 50f + (chargeTime * 20f);  // Example: more charge = more damage
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);  // Apply the damage
            }
        }
    }

    void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Enemy"))
    {
        Health enemyHealth = other.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);

            Rigidbody enemyRb = other.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockbackDirection * 5f, ForceMode.Impulse);
            }
        }
    }
}



}
