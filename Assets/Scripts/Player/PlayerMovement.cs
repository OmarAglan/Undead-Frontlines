using UnityEngine;
using UnityEngine.InputSystem; // Import the new Input System namespace

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // References
    private CharacterController controller;
    private PlayerControls playerControls; // Generated C# class
    private Animator animator; // We'll add this later

    // Input Values
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintInput;
    // ... other input flags will be added later

    // Movement Parameters (Use values from document)
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float crouchSpeed = 2f;
    public float proneSpeed = 1f;
    private float currentSpeed; // Will hold the actual speed based on state

    // Stamina Parameters (From document)
    public float maxStamina = 100f; // Represents 10 seconds sprint (10 units/sec)
    public float stamina = 100f;
    public float sprintStaminaCost = 10f; // per second
    public float diveStaminaCost = 20f; // per action
    public float staminaRegenRate = 5f; // per second
    public float staminaRegenDelay = 1.0f; // Time after stopping cost before regen starts
    private float timeSinceStaminaUsed = 0f;
    private bool isRegeneratingStamina = false;

    // Camera Control
    public Transform cameraFollowTarget; // Assign an empty GameObject child of Player
    public float lookSensitivity = 1.0f;
    private float cameraPitch = 0.0f;

    // Gravity
    private float gravity = -9.81f;
    private float verticalVelocity = 0.0f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask; // Set this in Inspector to your ground layer
    private bool isGrounded;

    // State Flags
    private bool isSprinting = false;
    // ... other state flags later

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>(); // Assuming Animator is on a child model

        playerControls = new PlayerControls();

        // --- Input Action Subscriptions ---
        playerControls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;

        playerControls.Gameplay.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Look.canceled += ctx => lookInput = Vector2.zero;

        playerControls.Gameplay.Sprint.performed += ctx => sprintInput = true;
        playerControls.Gameplay.Sprint.canceled += ctx => sprintInput = false;

        // ... subscribe to other actions (Crouch, Prone, Dive, Climb, Aim) here later

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        playerControls.Gameplay.Disable();
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleGravity();
        HandleRotation();
        HandleMovement();
        HandleStaminaRegen();
        // Handle other states (Crouch, Prone, etc.) will be added here
    }

    private void HandleGroundCheck()
    {
        // Simple sphere cast downwards
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, controller.height / 2 - controller.radius, 0), controller.radius - 0.1f, groundMask);
        // A slightly more robust check:
        // isGrounded = Physics.SphereCast(transform.position + Vector3.up * controller.radius, controller.radius * 0.9f, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask);

        // Reset vertical velocity if grounded
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small downward force to keep grounded
        }
    }

    private void HandleGravity()
    {
        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        // Player body rotation (Yaw) based on horizontal look input
        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity * Time.deltaTime);

        // Camera pitch (Vertical look) - Clamp to prevent flipping
        cameraPitch -= lookInput.y * lookSensitivity * Time.deltaTime;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f); // Prevent camera from flipping over

        // Apply pitch to the camera follow target
        if (cameraFollowTarget != null)
        {
            cameraFollowTarget.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        }
        else
        {
            Debug.LogWarning("Camera Follow Target not assigned in PlayerMovement script.");
        }
    }

    private void HandleMovement()
    {
        // Determine current speed based on state (basic example)
        isSprinting = sprintInput && moveInput.y > 0 && isGrounded; // Only sprint forward and when grounded (and has stamina later)
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        // Check if trying to sprint and has stamina
        bool canSprint = sprintInput && moveInput.y > 0 && isGrounded && stamina > 0;

        // Update isSprinting state AFTER checking if allowed
        isSprinting = canSprint;

        // Determine current speed based on state
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // We will add crouch/prone speed logic later

        // Calculate move direction based on input and player forward/right axes
        Vector3 moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;

        // Apply movement
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        // Consume Stamina if sprinting
        if (isSprinting)
        {
            ConsumeStamina(sprintStaminaCost * Time.deltaTime);
        }

        // --- Animation Parameter Update (Example) ---
        // if (animator != null)
        // {
        //     float animationSpeed = moveInput.magnitude * (isSprinting ? 2f : 1f); // Example blend value
        //     animator.SetFloat("Speed", animationSpeed); // Assuming a "Speed" float parameter in Animator
        //     animator.SetBool("IsGrounded", isGrounded);
        // }
    }
    private void ConsumeStamina(float amount)
    {
        if (stamina > 0)
        {
            stamina = Mathf.Max(stamina - amount, 0); // Prevent negative stamina
            timeSinceStaminaUsed = 0f; // Reset regen delay timer
            isRegeneratingStamina = false;
            // Update UI if you have one
        }
        // Stop sprint immediately if stamina hits zero
        if (stamina <= 0)
        {
            isSprinting = false;
        }
    }

    private void HandleStaminaRegen()
    {
        // Increment the timer if stamina isn't full and hasn't been used recently
        if (stamina < maxStamina && !isSprinting /* && !isDiving (add later) */)
        {
            timeSinceStaminaUsed += Time.deltaTime;
        }

        // Start regenerating after the delay
        if (timeSinceStaminaUsed >= staminaRegenDelay && stamina < maxStamina)
        {
            isRegeneratingStamina = true;
            stamina = Mathf.Min(stamina + staminaRegenRate * Time.deltaTime, maxStamina);
            // Update UI if you have one
        }
        else
        {
            isRegeneratingStamina = false;
        }
    }

    // Helper function to check if player has enough stamina
    public bool HasEnoughStamina(float requiredAmount)
    {
        return stamina >= requiredAmount;
    }

    // --- We will add methods for Crouch, Prone, Dive, Climb later ---
}