using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TelekinesisControl : MonoBehaviour
{
    [Header("Telekinesis Settings")]
    public Transform playerCamera;
    public float pickUpDistance = 100f;
    public float holdDistance = 3f;
    public float moveSpeed = 10f;
    public float pullSpeed = 5f;
    public float throwForceMultiplier = 10f;  // For charged throws
    public float quickThrowForce = 20f;       // New: For quick throws when pressing F
    public float multiPickUpRadius = 5f;

    [Header("Crosshair Settings")]
    public Image crosshair;
    public Color defaultCrosshairColor = Color.white;
    public Color targetCrosshairColor = Color.red;

    [Header("Sound Settings")]
    public AudioClip pickUpSound;       // Sound when picking up an object
    public AudioClip collisionSound;    // Sound when object collides
    public float collisionVolume = 0.5f;
    public float pickUpVolume = 0.6f;

    private AudioSource audioSource;

    private GameObject heldObject;
    private bool isHolding = false;
    private GameObject highlightedObject;
    private List<GameObject> heldObjects = new List<GameObject>(); 
    private float chargeTime = 0f; 
    public float maxChargeTime = 3f; 
 
    private Color lightGray = new Color(0.8f, 0.8f, 0.8f);
    private Color darkGray = new Color(0.3f, 0.3f, 0.3f);

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }

        if (crosshair != null)
        {
            crosshair.color = defaultCrosshairColor;
        }

        // Add an AudioSource to the player if none exists
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }

    void Update()
    {
        DetectTeleObjects();
        HandlePickUpAndRelease();
        HandleProjectileTelekinesis();
        HandleMultiObjectTelekinesis();
    }

    // Detect teleObjects only in the direction of the crosshair
   void DetectTeleObjects()
{
    // Always reset previously highlighted object
    if (highlightedObject != null)
    {
        SetObjectColor(highlightedObject, lightGray);
        highlightedObject = null;  // ✅ Clear reference to prevent lingering highlight
    }

    Ray ray = new Ray(playerCamera.position, playerCamera.forward);
    RaycastHit hit;

    // If raycast hits an object within range
    if (Physics.Raycast(ray, out hit, pickUpDistance))
    {
        if (hit.collider.CompareTag("teleObject"))
        {
            highlightedObject = hit.collider.gameObject;
            SetObjectColor(highlightedObject, darkGray);

            if (crosshair != null)
            {
                crosshair.color = targetCrosshairColor;
            }
        }
        else
        {
            ResetCrosshair(); // If the hit object is not "teleObject", reset crosshair
        }
    }
    else
    {
        ResetCrosshair(); // No object detected, reset crosshair
    }
}


    void SetObjectColor(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }

    void ResetCrosshair()
    {
        if (crosshair != null)
        {
            crosshair.color = defaultCrosshairColor;
        }
    }

    // Picking up and releasing objects with E
    void HandlePickUpAndRelease()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isHolding)
            {
                ReleaseObject();
            }
            else
            {
                TryPickUpObject();
            }
        }

        if (isHolding && heldObject != null)
        {
            HoldObject();
        }

if (isHolding && heldObject != null)
{
    float smoothSpeed = 2f;  // Adjust for smoother motion
    float decayFactor = Mathf.Clamp(1f - (Time.timeSinceLevelLoad * 0.01f), 0.2f, 1f);

    // Base Randomized Rotation (constant rotation effect)
    Vector3 randomRotation = new Vector3(
        Random.Range(30f, 80f),  // X-axis rotation
        Random.Range(80f, 150f), // Y-axis rotation
        Random.Range(40f, 100f)  // Z-axis rotation
    );

    // Floating Effect using Sin/Cos waves
    Vector3 floatingRotation = new Vector3(
        Mathf.Sin(Time.time) * 50f,  // Smooth X oscillation
        Mathf.Cos(Time.time) * 75f,  // Smooth Y oscillation
        Mathf.Sin(Time.time * 1.5f) * 60f // Smooth Z oscillation
    );

    // Apply both random rotation and smooth oscillation
    heldObject.transform.Rotate((randomRotation + floatingRotation) * Time.deltaTime * smoothSpeed * decayFactor);
}



    }

void TryPickUpObject()
{
    if (highlightedObject != null)
    {
        Debug.Log("Picking up: " + highlightedObject.name);
        heldObject = highlightedObject;
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        isHolding = true;

        // ✅ Play pickup sound correctly
        if (pickUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickUpSound, pickUpVolume);
        }
        else
        {
            Debug.LogWarning("Pickup sound missing or AudioSource not set!");
        }
            // Attach collision sound script
            if (heldObject.GetComponent<TelekinesisCollision>() == null)
            {
                heldObject.AddComponent<TelekinesisCollision>().collisionSound = collisionSound;
            }


    }
    else
    {
        Debug.Log("No object to pick up!");
    }
}


    void HoldObject()
{
    // Ensure the held object stays in front of Silver's model (player)
    Vector3 desiredPosition = transform.position + transform.forward * holdDistance + Vector3.up * 1.5f; // Slightly above ground level for realism
    Rigidbody rb = heldObject.GetComponent<Rigidbody>();
    Vector3 direction = desiredPosition - heldObject.transform.position;
    rb.linearVelocity = direction * moveSpeed;  // Smooth movement towards the hold position
}

void ReleaseObject()
{
    if (heldObject != null)
    {
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.freezeRotation = false;

        // ✅ Stop pickup sound when releasing
        AudioSource objAudioSource = heldObject.GetComponent<AudioSource>();
        if (objAudioSource != null)
        {
            objAudioSource.Stop();
            objAudioSource.loop = false; // Ensure it doesn't loop after stopping
        }

        SetObjectColor(heldObject, lightGray); // ✅ Unhighlight when released

        heldObject = null;
        isHolding = false;
    }
}



    // Updated Telekinesis Logic: Press F to throw when holding, otherwise pull and charge
    void HandleProjectileTelekinesis()
{
    if (Input.GetKeyDown(KeyCode.F))
    {
        if (isHolding)
        {
            QuickThrowObject();  // Quick throw if holding
        }
        else
        {
            TryPullObject();  // Start pulling if not holding
        }
    }

    if (Input.GetKey(KeyCode.F) && isHolding)
    {
        chargeTime += Time.deltaTime;
        chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime);
    }

    if (Input.GetKeyUp(KeyCode.F) && isHolding)
    {
        ThrowObject();  // Release charged throw
    }
}


    void TryPullObject()
    {
        if (highlightedObject != null)
        {
            heldObject = highlightedObject;
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.freezeRotation = true;

            Vector3 pullDirection = playerCamera.position - heldObject.transform.position;
            rb.linearVelocity = pullDirection.normalized * pullSpeed;
        }
    }

    // New Method: Quick throw when pressing F while holding the object


    // Original Charged Throw
void QuickThrowObject()
{
    Rigidbody rb = heldObject.GetComponent<Rigidbody>();
    rb.useGravity = true;
    rb.freezeRotation = false;

    // Apply force in the camera's forward direction
    rb.AddForce(playerCamera.forward * quickThrowForce, ForceMode.VelocityChange);

    SetObjectColor(heldObject, lightGray);
    heldObject = null;
    isHolding = false;
}



void ThrowObject()
{
    Rigidbody rb = heldObject.GetComponent<Rigidbody>();
    rb.useGravity = true;
    rb.freezeRotation = false;

    float throwForce = (chargeTime / maxChargeTime) * throwForceMultiplier;

    // Apply charged force in the direction the camera is facing
    rb.AddForce(playerCamera.forward * throwForce, ForceMode.Impulse);

    // Apply damage based on charge time
    TelekinesisDamage damageScript = heldObject.GetComponent<TelekinesisDamage>();
    if (damageScript != null)
    {
        damageScript.SetDamage(chargeTime);
    }

    SetObjectColor(heldObject, lightGray);

    heldObject = null;
    isHolding = false;
    chargeTime = 0f;
}


    // Multi-Object Telekinesis (F + Right Mouse Button)
    void HandleMultiObjectTelekinesis()
    {
        if (Input.GetKey(KeyCode.F) && Input.GetMouseButtonDown(1))
        {
            MultiPickUpObjects();
        }

        if (heldObjects.Count > 0)
        {
            UpdateHeldObjects();
        }
    }

void MultiPickUpObjects()
{
    Collider[] objectsInRange = Physics.OverlapSphere(playerCamera.position, multiPickUpRadius);
    foreach (var obj in objectsInRange)
    {
        if (obj.CompareTag("teleObject") && !heldObjects.Contains(obj.gameObject))
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.freezeRotation = true;

            heldObjects.Add(obj.gameObject);

            // Ignore collisions between picked-up objects
            foreach (var otherObj in heldObjects)
            {
                if (otherObj != obj.gameObject)
                {
                    Physics.IgnoreCollision(obj.GetComponent<Collider>(), otherObj.GetComponent<Collider>());
                }
            }
        }
    }
}


    void UpdateHeldObjects()
    {
        foreach (GameObject obj in heldObjects)
        {
            Vector3 desiredPosition = playerCamera.position + Random.onUnitSphere * 2f;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            Vector3 direction = desiredPosition - obj.transform.position;
            rb.linearVelocity = direction * moveSpeed;
        }
    }
}