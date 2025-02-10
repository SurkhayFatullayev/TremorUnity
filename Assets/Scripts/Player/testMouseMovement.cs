using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivityV = 100f; // Sensitivity for vertical mouse movement
    public float mouseSensitivityH = 100f; // Sensitivity for horizontal mouse movement

    private float xRotation = 0f; // Tracks vertical rotation (pitch)

    public Transform playerBodyH; // Reference to the player's body for horizontal rotation
    public Transform playerBodyV; // Reference to the camera or object for vertical rotation

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor to the center of the screen
    }

    // Update is called once per frame
    void Update()
    {
        // Get horizontal and vertical mouse inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityH * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityV * Time.deltaTime;

        // Adjust vertical rotation (pitch) and clamp it
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply vertical rotation to the playerBodyV (e.g., camera)
        playerBodyV.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply horizontal rotation to the playerBodyH (e.g., character body)
        playerBodyH.Rotate(Vector3.up * mouseX);
    }
}
