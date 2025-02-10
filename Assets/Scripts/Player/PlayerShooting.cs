using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(firePoint.forward * projectileSpeed, ForceMode.Impulse);
        }

        Destroy(projectile, 3f);
    }
}
