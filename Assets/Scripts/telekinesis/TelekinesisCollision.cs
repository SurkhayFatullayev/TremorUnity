using UnityEngine;

public class TelekinesisCollision : MonoBehaviour
{
    public AudioClip collisionSound;  // Sound when object hits something
    private AudioSource audioSource;

    void Start()
    {
        // Add AudioSource if it doesn’t exist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ensure we don’t play sound for minor bounces
        if (collision.relativeVelocity.magnitude > 2f) 
        {
            audioSource.PlayOneShot(collisionSound, 0.7f);
        }
    }
}
