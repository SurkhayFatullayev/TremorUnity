using UnityEngine;

public class RaycastShooting : MonoBehaviour
{
    public float baseDamage;
    public float range;
    public float fireRate;
    public float bulletSpread;
    public Transform firePoint;
    public GameObject hitEffectPrefab;
    public GameObject tracerPrefab;
    public bool visualizeRay = false;
    public bool isAutomatic = false;

    private float nextFireTime = 0f;
    private Shooter shooter;

    [Header("Player Movement Reference")]
    public FirstPersonController playerMovement;  // Reference to FirstPersonController

    private float playerSpeed;


    void Start()
    {
        shooter = new Shooter(baseDamage, range, bulletSpread, firePoint, hitEffectPrefab, tracerPrefab, visualizeRay);
        // If playerMovement has a velocity variable, use it
        if (playerMovement != null && playerMovement.velocity != null)
        {
            playerSpeed = playerMovement.velocity.magnitude;
        }
        else
        {
            // If no velocity is available, manually calculate speed
            playerSpeed = 0f;
        }
    }

    void Update()
    {
        if ((isAutomatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1")) && Time.time >= nextFireTime)
        {
            shooter.Shoot(playerMovement.velocity);  // Pass the velocity to the Shoot method
            nextFireTime = Time.time + 1f / fireRate;
        }
    }
}

public class Shooter
{
    private float baseDamage;
    private float range;
    public float bulletSpread;
    private Transform firePoint;
    private GameObject hitEffectPrefab;
    private GameObject tracerPrefab;
    private bool visualizeRay;
    private System.Random random;

    public Shooter(float baseDamage, float range, float bulletSpread, Transform firePoint, GameObject hitEffectPrefab, GameObject tracerPrefab, bool visualizeRay)
    {
        this.baseDamage = baseDamage;
        this.range = range;
        this.bulletSpread = bulletSpread;
        this.firePoint = firePoint;
        this.hitEffectPrefab = hitEffectPrefab;
        this.tracerPrefab = tracerPrefab;
        this.visualizeRay = visualizeRay;
        this.random = new System.Random();
    }

    public void Shoot(Vector3 playerVelocity)
    {
        // Use velocity magnitude to multiply the bullet spread
        float spreadMultiplier = playerVelocity.magnitude;  // Get the magnitude (speed)
        // Generate a more controlled spread
        Vector3 spread = new Vector3(
            (float)(random.NextDouble() - 0.5) * bulletSpread * spreadMultiplier,
            (float)(random.NextDouble() - 0.5) * bulletSpread * spreadMultiplier,
            0);

        Vector3 rayDirection = (firePoint.forward + spread).normalized;
        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, rayDirection, out hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // Apply damage if the target has a Health script
            Health enemy = hit.collider.GetComponent<Health>();
            if (enemy != null)
            {
                enemy.TakeDamage(baseDamage);
            }

            // Spawn hit effect
            if (hitEffectPrefab != null)
            {
                GameObject hitEffect = Object.Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Object.Destroy(hitEffect, 3f);
            }

            // Spawn tracer effect
            if (tracerPrefab != null)
            {
                GameObject tracer = Object.Instantiate(tracerPrefab, firePoint.position, Quaternion.LookRotation(rayDirection));
                Object.Destroy(tracer, 0.5f);
            }
        }

        // Visualize the ray
        if (visualizeRay)
        {
            Debug.DrawRay(firePoint.position, rayDirection * range, Color.red, 10f);
        }
    }
}
