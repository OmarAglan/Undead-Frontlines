using UnityEngine;
using UnityEngine.InputSystem; // Required for New Input System

// Require necessary components
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))] // Required for Input System integration
public class PlayerMovement : MonoBehaviour
{
    // --- Input Action References (Assign in Inspector) ---
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;
    // Add references for Dive, Prone, Climb later

    // --- Public Editable Variables (Inspector) ---
    [Header("Movement Speeds")]
    [Tooltip("Base walking speed in meters per second.")]
    public float walkSpeed = 4f;
    [Tooltip("Sprinting speed multiplier based on walk speed.")]
    public float sprintMultiplier = 1.5f;
    [Tooltip("Crouching speed multiplier based on walk speed.")]
    public float crouchMultiplier = 0.5f;
    [Tooltip("Prone speed multiplier based on walk speed.")]
    public float proneMultiplier = 0.25f; // Not implemented yet

    [Header("Look Settings (Sensitivity handled by Input System/Cinemachine)")]
    [Tooltip("Multiplier for horizontal rotation speed of the player body.")]
    public float bodyRotationSensitivity = 1.0f; // Adjust how fast the body turns

    [Header("Stamina Settings")]
    [Tooltip("Maximum stamina capacity.")]
    public float maxStamina = 100f;
    [Tooltip("Stamina consumed per second while sprinting.")]
    public float sprintStaminaCost = 10f;
    [Tooltip("Stamina consumed per dive action.")]
    public float diveStaminaCost = 20f; // Not implemented yet
    [Tooltip("Stamina regenerated per second when not consuming.")]
    public float staminaRegenRate = 5f;
    [Tooltip("Delay in seconds before stamina starts regenerating after consumption.")]
    public float staminaRegenDelay = 1.5f;

    [Header("Physics & Ground Check")]
    [Tooltip("Force of gravity applied to the player.")]
    public float gravity = -19.62f;
    [Tooltip("Distance to check for ground beneath the player.")]
    public float groundCheckDistance = 0.2f; // Adjusted slightly
    [Tooltip("Layer mask to determine what is considered ground.")]
    public LayerMask groundMask;
    [Tooltip("Offset for the ground check sphere from the player's bottom.")]
    public float groundCheckOffset = 0.1f;

    [Header("Crouching")]
    [Tooltip("Height of the CharacterController when standing.")]
    public float standingHeight = 1.8f;
    [Tooltip("Height of the CharacterController when crouching.")]
    public float crouchingHeight = 0.9f;
    [Tooltip("Time it takes to transition between standing and crouching.")]
    public float crouchTransitionSpeed = 10f;

    // --- Private Variables ---
    private CharacterController characterController;
    private Animator animator; // Get this if you have animations
    // Camera is now handled by Cinemachine, no direct reference needed for rotation

    private Vector3 velocity; // Player's current vertical velocity (for gravity)
    private bool isGrounded;

    private float currentStamina;
    private float timeSinceLastStaminaUse = 0f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintInputPressed;
    // Crouch input is handled via callbacks (OnCrouch)

    private float targetHeight;
    private Vector3 targetCenter;
    private float standingCenterY;
    private float crouchingCenterY;

    // --- Movement State ---
    private enum MovementState { Walking, Sprinting, Crouching, Prone, Diving, Climbing } // Prone, Diving, Climbing not implemented
    private MovementState currentState = MovementState.Walking;

    // --- Initialization ---
    void Awake()
    {
        // Get required components
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>(); // Optional: Get the animator if present

        // Lock and hide cursor (optional, can be handled elsewhere)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize stamina
        currentStamina = maxStamina;

        // Initialize height/center targets
        targetHeight = standingHeight;
        standingCenterY = characterController.center.y; // Assumes initial setup is standing
        crouchingCenterY = standingCenterY - (standingHeight - crouchingHeight) / 2f;
        targetCenter = characterController.center;

        // Ensure CharacterController starts with correct values
        characterController.height = standingHeight;
        characterController.center = new Vector3(characterController.center.x, standingCenterY, characterController.center.z);
    }

    // --- Enable/Disable Input Actions ---
    private void OnEnable()
    {
        // Enable actions referenced in the inspector
        moveAction.action.Enable();
        lookAction.action.Enable();
        sprintAction.action.Enable();
        crouchAction.action.Enable();

        // Subscribe to crouch action events (for toggle-like behavior)
        crouchAction.action.performed += OnCrouch;
    }

    private void OnDisable()
    {
        // Disable actions to prevent errors when object is disabled
        moveAction.action.Disable();
        lookAction.action.Disable();
        sprintAction.action.Disable();
        crouchAction.action.Disable();

        // Unsubscribe from events
        crouchAction.action.performed -= OnCrouch;
    }

    // --- Update Loop (Called Every Frame) ---
    void Update()
    {
        // --- Ground Check ---
        CheckGrounded();

        // --- Input Reading ---
        ReadInput(); // Read values from enabled actions

        // --- Camera Look (Body Rotation Only) ---
        HandleLook(); // Rotates the player body horizontally

        // --- State & Movement Logic ---
        HandleStateTransitions(); // Determine current state (Walking, Sprinting, Crouching)
        HandleMovement();       // Apply movement based on state

        // --- Stamina Management ---
        HandleStamina();

        // --- Apply Gravity ---
        ApplyGravity();

        // --- Handle Height Transitions (Crouch) ---
        HandleCrouchTransition();

        // --- Update Animator (Placeholder) ---
        // UpdateAnimator();
    }

    // --- Input Reading Function ---
    void ReadInput()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        lookInput = lookAction.action.ReadValue<Vector2>();
        sprintInputPressed = sprintAction.action.IsPressed();
        // Crouch input is handled by the OnCrouch callback
    }

     // --- Crouch Input Callback ---
    private void OnCrouch(InputAction.CallbackContext context)
    {
        // Toggle crouch state
        if (currentState == MovementState.Crouching)
        {
            // Stand up only if there's space
            if (CanStandUp())
            {
                 currentState = MovementState.Walking; // Or previous non-crouch state
            }
        }
        else if (isGrounded && currentState != MovementState.Sprinting) // Don't crouch while sprinting for now
        {
            currentState = MovementState.Crouching;
        }
    }

    // --- Camera Look / Body Rotation Function ---
    void HandleLook()
    {
        // Rotate Player Body Horizontally based on Look Input X
        // Sensitivity is now partly controlled by the Input Action's settings,
        // but we can add a multiplier here.
        float horizontalRotation = lookInput.x * bodyRotationSensitivity * Time.deltaTime * 100f; // Multiply by 100 for sensitivity similar to old system
        transform.Rotate(Vector3.up * horizontalRotation);

        // Vertical camera rotation is handled by Cinemachine using Look Input Y
    }

    // --- Determine Current Movement State ---
    void HandleStateTransitions()
    {
        // Sprinting Check (Overrides Walking if conditions met)
        if (sprintInputPressed && isGrounded && currentStamina > 0 && moveInput.magnitude > 0.1f && currentState != MovementState.Crouching)
        {
            currentState = MovementState.Sprinting;
        }
        // Revert from Sprinting if button released, no stamina, not moving, or crouch toggled
        else if (currentState == MovementState.Sprinting && (!sprintInputPressed || currentStamina <= 0 || moveInput.magnitude < 0.1f))
        {
            currentState = MovementState.Walking; // Revert to walking
        }
        // If not Sprinting or Crouching, default to Walking
        else if (currentState != MovementState.Crouching && currentState != MovementState.Sprinting)
        {
            currentState = MovementState.Walking;
        }

        // Note: Crouch state is toggled in OnCrouch callback
    }


    // --- Movement Logic Function ---
    void HandleMovement()
    {
        // Determine target speed based on state
        float currentSpeedMultiplier = 1f;
        switch (currentState)
        {
            case MovementState.Walking:
                currentSpeedMultiplier = 1f;
                break;
            case MovementState.Sprinting:
                currentSpeedMultiplier = sprintMultiplier;
                break;
            case MovementState.Crouching:
                currentSpeedMultiplier = crouchMultiplier;
                break;
            // Add Prone later
        }
        float targetSpeed = walkSpeed * currentSpeedMultiplier;

        // Create movement vector based on input and player's forward/right direction
        // Already normalized by Input System for Vector2, but check magnitude
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Apply movement using CharacterController
        characterController.Move(moveDirection * targetSpeed * Time.deltaTime);
    }

     // --- Crouch Height Transition ---
    void HandleCrouchTransition()
    {
        targetHeight = (currentState == MovementState.Crouching) ? crouchingHeight : standingHeight;
        targetCenter.y = (currentState == MovementState.Crouching) ? crouchingCenterY : standingCenterY;

        // Smoothly interpolate height and center
        float newHeight = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        Vector3 newCenter = Vector3.Lerp(characterController.center, targetCenter, Time.deltaTime * crouchTransitionSpeed);

        // Apply interpolated values
        characterController.height = newHeight;
        characterController.center = newCenter;
    }

    // --- Check if player can stand up from crouch ---
    bool CanStandUp()
    {
        // Cast a capsule upwards from the crouched position to see if it hits anything
        float radius = characterController.radius;
        // Start point slightly above the crouched center
        Vector3 startPoint = transform.position + new Vector3(0, crouchingCenterY + radius - 0.1f, 0);
         // End point at the standing height center
        Vector3 endPoint = transform.position + new Vector3(0, standingCenterY - radius + 0.1f, 0);
        float castDistance = (standingHeight - crouchingHeight); // Approximate distance needed

        // Check for obstructions (ignore the player itself)
        // Use CapsuleCast for better accuracy matching the controller shape
        return !Physics.CapsuleCast(startPoint, endPoint, radius, Vector3.up, castDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
         // Note: Ensure your player GameObject is on the "Player" layer or adjust the layer mask accordingly.
    }


    // --- Stamina Management Function ---
    void HandleStamina()
    {
        bool isConsumingStamina = false;

        // Consume stamina if sprinting and moving
        if (currentState == MovementState.Sprinting && moveInput.magnitude > 0.1f)
        {
            currentStamina -= sprintStaminaCost * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            timeSinceLastStaminaUse = 0f; // Reset regen delay timer
            isConsumingStamina = true;
            // No need to force walk here, HandleStateTransitions handles it if stamina runs out
        }

        // Placeholder for Dive stamina cost

        // Regenerate stamina if not consuming and delay has passed
        if (!isConsumingStamina && currentStamina < maxStamina)
        {
            timeSinceLastStaminaUse += Time.deltaTime;
            if (timeSinceLastStaminaUse >= staminaRegenDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            }
        }

        // --- Optional: Update UI ---
        // UpdateStaminaUI(currentStamina / maxStamina);
    }

    // --- Gravity Application ---
    void ApplyGravity()
    {
        // Reset vertical velocity if grounded and moving downwards
        if (isGrounded && velocity.y < 0)
        {
            // Setting velocity.y to a small negative value helps keep the controller grounded.
            // Avoid setting to 0 directly, as it might cause slight bouncing.
            velocity.y = -2f;
        }

        // Apply gravity over time
        velocity.y += gravity * Time.deltaTime;

        // Apply vertical movement (gravity) using Move.
        // Move is frame rate independent due to Time.deltaTime.
        characterController.Move(velocity * Time.deltaTime);
    }

    // --- Ground Check Function ---
    void CheckGrounded()
    {
        // Calculate sphere position slightly below the controller's bottom center
        Vector3 spherePosition = transform.position + characterController.center;
        spherePosition.y = transform.position.y + characterController.radius - groundCheckOffset; // Start check from radius height downwards

        // Perform a sphere check
        isGrounded = Physics.CheckSphere(spherePosition, characterController.radius, groundMask, QueryTriggerInteraction.Ignore);

        // Debug Draw Sphere (Optional)
        #if UNITY_EDITOR
        debugSphereColor = isGrounded ? Color.green : Color.red;
        debugSpherePos = spherePosition;
        debugSphereRadius = characterController.radius;
        #endif
    }


    // --- Placeholder for Animator Updates ---
    // void UpdateAnimator()
    // {
    //     // Get current horizontal speed
    //     Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
    //     float currentSpeed = horizontalVelocity.magnitude;
    //     float normalizedSpeed = currentSpeed / (walkSpeed * sprintMultiplier); // Normalize based on max potential speed
    //
    //     // Send parameters to Animator
    //     animator.SetFloat("Speed", normalizedSpeed); // Send normalized speed (0-1)
    //     animator.SetBool("IsSprinting", currentState == MovementState.Sprinting);
    //     animator.SetBool("IsCrouching", currentState == MovementState.Crouching);
    //     // animator.SetBool("IsProne", currentState == MovementState.Prone); // Add later
    //     animator.SetBool("IsGrounded", isGrounded);
    //     // Add triggers for Dive, Climb etc. (e.g., animator.SetTrigger("Dive"))
    // }

    // --- Placeholder for UI Update ---
    // void UpdateStaminaUI(float normalizedStamina)
    // {
    //     // Example: if (staminaBarSlider != null) staminaBarSlider.value = normalizedStamina;
    // }

    // --- Gizmos for Debugging (Visible in Scene View) ---
    #if UNITY_EDITOR
    private Color debugSphereColor = Color.red;
    private Vector3 debugSpherePos;
    private float debugSphereRadius;
    void OnDrawGizmosSelected()
    {
        // Draw Ground Check sphere using stored values
        if (characterController != null) // Check if controller exists
        {
            Gizmos.color = debugSphereColor;
            Gizmos.DrawWireSphere(debugSpherePos, debugSphereRadius);
        }
    }
    #endif
}
