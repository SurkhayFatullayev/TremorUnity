using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public int NumberOfCores { get; private set; }
    public UnityEvent<PlayerInventory> OnPowerCoreCollected;
    
    [SerializeField] private AudioClip collectionSound; // Reference to an AudioClip (MP3, WAV, etc.)
    private AudioSource audioSource; // Internal AudioSource

    private void Start()
    {
        // Get or Add an AudioSource to the GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Create one if missing
        }
    }

    public void CoreCollected()
    {
        NumberOfCores++;
        OnPowerCoreCollected.Invoke(this);

        // Play the sound when a Power Core is collected
        if (collectionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectionSound);
        }
        else
        {
            Debug.LogWarning("No AudioClip assigned for Power Core collection!");
        }
    }
}
