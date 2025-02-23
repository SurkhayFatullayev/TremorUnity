using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TelekinesisControl : MonoBehaviour
{
    #region Telekinesis Settings
    public Transform playerCamera;
    public float pickUpDistance = 100f;
    public float holdDistance = 3f;
    public float moveSpeed = 10f;
    public float pullSpeed = 5f;
    public float throwForceMultiplier = 10f;  // For charged throws
    public float quickThrowForce = 20f;       // New: For quick throws when pressing F
    public float multiPickUpRadius = 5f;
    #endregion
    #region Crosshair Settings
    public Image crosshair;
    public Color defaultCrosshairColor = Color.white;
    public Color targetCrosshairColor = Color.red;

    private GameObject heldObject;
    private bool isHolding = false;
    private GameObject highlightedObject;
    private List<GameObject> heldObjects = new List<GameObject>(); 
    private float chargeTime = 0f; 
    public float maxChargeTime = 3f; 
 
    private Color lightGray = new Color(0.8f, 0.8f, 0.8f);
    private Color darkGray = new Color(0.3f, 0.3f, 0.3f);
    #endregion

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
    }

    void Update()
    {
        DetectTeleObjects();
        HandlePickUpAndRelease();
        HandleProjectileTelekinesis();
    }

    // Detect teleObjects only in the direction of the crosshair
    void DetectTeleObjects()
    {
        if (highlightedObject != null && !isHolding)
        {
            SetObjectColor(highlightedObject, lightGray);
            highlightedObject = null;
        }

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

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
                ResetCrosshair();
            }
        }
        else
        {
            ResetCrosshair();
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
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.freezeRotation = false;

        SetObjectColor(heldObject, lightGray);

        heldObject = null;
        isHolding = false;
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

            Vector3 pullDirection = playerCamera.position - 2*heldObject.transform.position;
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
        damageScript.SetDamage(50);
    }

    SetObjectColor(heldObject, lightGray);

    heldObject = null;
    isHolding = false;
    chargeTime = 0f;
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
