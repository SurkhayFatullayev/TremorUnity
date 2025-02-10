using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6.0f; // Movement speed
    public float gravity = -9.8f; // Gravity force
    public float jumpHeight = 2.0f; // Jump strength
    public float terminalVelocity = -50f; // Max fall speed
    public float acceleration = 10.0f; // How quickly the character reaches target speed
    public float deceleration = 5.0f; // How quickly the character slows down when stopping

    private CharacterController controller;
    private Vector3 velocity; // Current vertical velocity
    private Vector3 currentMovement; // Current horizontal velocity
    private Vector3 targetMovement; // Target horizontal velocity
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if the player is on the ground
        isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            velocity.y = -0.1f; // Reset downward velocity
        }

        // Get input for movement (WASD/Arrow Keys)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calculate target movement direction and speed
        targetMovement = (transform.right * moveX + transform.forward * moveZ) * speed;

        // Smoothly interpolate the current movement toward the target movement
        float interpolationSpeed = targetMovement.magnitude > 0 ? acceleration : deceleration;
        currentMovement = Vector3.Lerp(currentMovement, targetMovement, interpolationSpeed * Time.deltaTime);

        // Apply horizontal movement
        controller.Move(currentMovement * Time.deltaTime);

        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Clamp to terminal velocity
        velocity.y = Mathf.Max(velocity.y, terminalVelocity);

        // Apply vertical movement
        controller.Move(velocity * Time.deltaTime);
    }
}
