using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("Audio Settings")]
    public AudioClip shootSound; // Sound effect for shooting
    public float shootVolume = 1.0f;
    private AudioSource audioSource;

    
        void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

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

        // Play shooting sound
        PlayShootSound();

        Destroy(projectile, 3f);
    }

    void PlayShootSound()
    {
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, shootVolume);
        }
        else
        {
            Debug.LogWarning("No shooting sound assigned!");
        }
    }
}
