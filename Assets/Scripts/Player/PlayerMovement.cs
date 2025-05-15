using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // Required for Cinemachine

// Require necessary components
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    // --- Input Action References (Assign in Inspector) ---
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference proneAction; // *** ADD THIS FOR PRONE ***
    [SerializeField] private InputActionReference diveAction; // *** ADD THIS FOR DIVE ***
    [SerializeField] private InputActionReference aimAction; // *** ADD THIS ***

    // --- Cinemachine References (Assign in Inspector) ---
    [Header("Cinemachine")]
    [Tooltip("The Virtual Camera used for aiming.")]
    [SerializeField] private CinemachineCamera aimVirtualCamera; // *** ADD THIS ***
    [Tooltip("Sensitivity multiplier for turning the player body while aiming.")]
    [SerializeField] private float aimingTurnSensitivity = 0.8f; // Adjust as needed
    [Tooltip("The main player camera (used to align player rotation when aiming). Assign the scene's Main Camera.")]
    [SerializeField] private Camera mainCamera; // *** ADD THIS ***

    // --- Public Editable Variables (Inspector) ---
    [Header("Movement Speeds")]
    public float walkSpeed = 4f;
    public float sprintMultiplier = 1.5f;
    public float crouchMultiplier = 0.5f;
    public float proneMultiplier = 0.25f;

    [Header("Look Settings")]
    public float bodyRotationSensitivity = 1.0f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float sprintStaminaCost = 10f;
    public float diveStaminaCost = 20f;
    public float staminaRegenRate = 5f;
    public float staminaRegenDelay = 1.5f;

    [Header("Physics & Ground Check")]
    public float gravity = -19.62f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;
    public float groundCheckOffset = 0.1f;

    [Header("Crouching")]
    public float standingHeight = 1.8f;
    public float crouchingHeight = 0.9f;
    public float crouchTransitionSpeed = 10f; // *** RE-ADD THIS VARIABLE ***
    [Header("Proning")] // *** ADD THIS FOR PRONE ***
    public float proneHeight = 0.4f; // *** ADD THIS FOR PRONE ***
    public float proneTransitionSpeed = 10f; // Can be same as crouch or different
    [Header("Diving")] // *** ADD THIS FOR DIVE ***
    public float diveForce = 8f;
    public float diveDuration = 0.5f; // How long the dive impulse lasts or player is in dive state

    // --- Private Variables ---
    private CharacterController characterController;
    private Animator animator;
    private PlayerInput playerInput; // Store reference to PlayerInput

    private Vector3 velocity;
    private bool isGrounded;
    private float currentStamina;
    private float timeSinceLastStaminaUse = 0f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintInputPressed;
    private bool aimInputPressed; // *** ADD THIS ***

    private float targetHeight;
    private Vector3 targetCenter;
    private float standingCenterY;
    private float crouchingCenterY;
    private float proneCenterY; // *** ADD THIS FOR PRONE ***
    private float diveTimer = 0f; // *** ADD THIS FOR DIVE ***

    // --- Movement State ---
    private enum MovementState { Walking, Sprinting, Crouching, Prone, Diving, Climbing, Aiming } // Prone was already here, good.
    private MovementState currentState = MovementState.Walking;
    private MovementState previousState = MovementState.Walking; // To return to after aiming

    // --- Initialization ---
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>(); // Get PlayerInput component

        // Assign main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main Camera not found. Please assign it in the inspector or ensure a camera is tagged 'MainCamera'.");
            }
        }


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentStamina = maxStamina;
        targetHeight = standingHeight;
        standingCenterY = characterController.center.y;
        // Calculate center points for different heights
        // Assuming character controller pivot is at the base for these calculations
        // If pivot is center, calculations might differ slightly for Y offset
        crouchingCenterY = standingCenterY - (standingHeight - crouchingHeight) / 2f;
        proneCenterY = standingCenterY - (standingHeight - proneHeight) / 2f; // *** ADD THIS FOR PRONE ***
        targetCenter = characterController.center;
        characterController.height = standingHeight;
        characterController.center = new Vector3(characterController.center.x, standingCenterY, characterController.center.z);

        // Ensure Aim camera is initially disabled
        if (aimVirtualCamera != null)
        {
            aimVirtualCamera.enabled = false;
        }
        else
        {
            Debug.LogError("Aim Virtual Camera is not assigned in the inspector!");
        }
    }

    // --- Enable/Disable Input Actions ---
    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        sprintAction.action.Enable();
        crouchAction.action.Enable();
        proneAction.action.Enable(); // *** ENABLE PRONE ACTION ***
        diveAction.action.Enable(); // *** ENABLE DIVE ACTION ***
        aimAction.action.Enable(); // *** ENABLE AIM ACTION ***

        // Subscribe to events
        crouchAction.action.performed += OnCrouch;
        proneAction.action.performed += OnProne; // *** SUBSCRIBE PRONE ACTION ***
        diveAction.action.performed += OnDive; // *** SUBSCRIBE DIVE ACTION ***
        aimAction.action.performed += OnAimPerformed; // *** SUBSCRIBE ***
        aimAction.action.canceled += OnAimCanceled;   // *** SUBSCRIBE ***
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        sprintAction.action.Disable();
        crouchAction.action.Disable();
        proneAction.action.Disable(); // *** DISABLE PRONE ACTION ***
        diveAction.action.Disable(); // *** DISABLE DIVE ACTION ***
        aimAction.action.Disable(); // *** DISABLE AIM ACTION ***

        // Unsubscribe from events
        crouchAction.action.performed -= OnCrouch;
        proneAction.action.performed -= OnProne; // *** UNSUBSCRIBE PRONE ACTION ***
        diveAction.action.performed -= OnDive; // *** UNSUBSCRIBE DIVE ACTION ***
        aimAction.action.performed -= OnAimPerformed; // *** UNSUBSCRIBE ***
        aimAction.action.canceled -= OnAimCanceled;   // *** UNSUBSCRIBE ***
    }

    // --- Update Loop ---
    void Update()
    {
        CheckGrounded();
        ReadInput();
        HandleLook(); // Now handles rotation differently based on aiming state
        HandleStateTransitions();
        HandleMovement();
        HandleStamina();
        ApplyGravity();
        HandleCrouchTransition();
        UpdateAnimator();
    }

    // --- Input Reading ---
    void ReadInput()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        lookInput = lookAction.action.ReadValue<Vector2>();
        sprintInputPressed = sprintAction.action.IsPressed();
        // Aim, Crouch, and Prone inputs are handled by callbacks
    }

    // --- Input Callbacks ---
    private void OnCrouch(InputAction.CallbackContext context)
    {
        if (currentState == MovementState.Aiming) return; // Don't toggle while aiming

        if (currentState == MovementState.Crouching) // If already crouching
        {
            if (CanStandUp(standingHeight, standingCenterY)) // Try to stand
            {
                currentState = MovementState.Walking;
            }
        }
        else if (currentState == MovementState.Prone) // If prone, try to go to crouch
        {
            if (CanStandUp(crouchingHeight, crouchingCenterY)) // Check if can transition to crouch
            {
                currentState = MovementState.Crouching;
            }
        }
        else if (isGrounded && currentState != MovementState.Sprinting) // If walking or other non-sprint/prone states
        {
            currentState = MovementState.Crouching;
        }
    }

    private void OnProne(InputAction.CallbackContext context) // *** ADD PRONE INPUT HANDLER ***
    {
        if (currentState == MovementState.Aiming) return; // Don't toggle while aiming

        if (currentState == MovementState.Prone) // If already prone
        {
            if (CanStandUp(crouchingHeight, crouchingCenterY)) // Try to go to crouch first
            {
                currentState = MovementState.Crouching;
            }
            // Optional: else if (CanStandUp(standingHeight, standingCenterY)) currentState = MovementState.Walking;
        }
        else if (isGrounded && (currentState == MovementState.Walking || currentState == MovementState.Crouching))
        {
            // Check if there's space to go prone (important!)
            // This check is simplified; a more robust check might be needed based on game feel
            if (CanGoProne())
            {
                currentState = MovementState.Prone;
            }
        }
    }

    private void OnDive(InputAction.CallbackContext context) // *** ADD DIVE INPUT HANDLER ***
    {
        // Conditions to dive:
        // 1. Must be grounded.
        // 2. Must have enough stamina.
        // 3. Must be in a state that allows diving (e.g., Walking or Sprinting).
        // 4. Player must have some forward input.
        if (isGrounded && currentStamina >= diveStaminaCost &&
            (currentState == MovementState.Walking || currentState == MovementState.Sprinting) &&
            moveInput.magnitude > 0.1f) // Check for actual movement input
        {
            currentState = MovementState.Diving;
            currentStamina -= diveStaminaCost;
            timeSinceLastStaminaUse = 0f; // Reset stamina regeneration delay
            diveTimer = diveDuration;     // Start the dive duration timer

            // Optional: Immediately set character height to prone or a specific dive height
            // This can help prevent getting stuck if the animation/movement doesn't quickly lower the player.
            // targetHeight = proneHeight;
            // targetCenter.y = proneCenterY;
            // characterController.height = proneHeight; // Snap height
            // characterController.center = new Vector3(characterController.center.x, proneCenterY, characterController.center.z); // Snap center

            // Animator will be updated in UpdateAnimator()
        }
    }

    private void OnAimPerformed(InputAction.CallbackContext context)
    {
        aimInputPressed = true;
        if (aimVirtualCamera != null) aimVirtualCamera.enabled = true; // Enable Aim Cam

        // Store the state we were in before aiming (unless already aiming)
        if (currentState != MovementState.Aiming)
        {
            previousState = currentState;
        }
        currentState = MovementState.Aiming;

        // Optional: Switch action map if you have separate controls for aiming
        // playerInput.SwitchCurrentActionMap("Aiming");
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        aimInputPressed = false;
        if (aimVirtualCamera != null) aimVirtualCamera.enabled = false; // Disable Aim Cam

        // Return to the state we were in before aiming
        currentState = previousState;

        // Optional: Switch back to the default action map
        // playerInput.SwitchCurrentActionMap("Gameplay");
    }

    // --- Camera Look / Body Rotation ---
    void HandleLook()
    {
        if (mainCamera == null) return; // Don't proceed if camera isn't set

        if (currentState == MovementState.Aiming)
        {
            // While aiming, rotate the player body to face the camera's forward direction
            // This provides more intuitive aiming control.
            Quaternion targetRotation = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0);
            // Smoothly rotate the player body towards the camera's direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * aimingTurnSensitivity * 10f); // Multiply for snappier rotation
        }
        else
        {
            // Default behavior: Rotate Player Body Horizontally based on Look Input X
            float horizontalRotation = lookInput.x * bodyRotationSensitivity * Time.deltaTime * 100f;
            transform.Rotate(Vector3.up * horizontalRotation);
        }
        // Vertical camera aiming is handled by Cinemachine based on Look Input Y
    }

    // --- State Transitions ---
    void HandleStateTransitions()
    {
        // Aiming and Diving states are primarily managed here or by their input callbacks

        // Handle Dive Completion
        if (currentState == MovementState.Diving)
        {
            diveTimer -= Time.deltaTime;
            if (diveTimer <= 0f)
            {
                // Transition to Prone after dive (or crouch if preferred)
                currentState = MovementState.Prone;
                // Ensure height is set correctly for prone after dive
                targetHeight = proneHeight;
                targetCenter.y = proneCenterY;
                // Snap height and center immediately for prone after dive
                characterController.height = proneHeight;
                characterController.center = new Vector3(characterController.center.x, proneCenterY, characterController.center.z);
            }
            return; // Stay in diving state until timer runs out
        }

        // If we are aiming, don't process other state changes for now (except for dive completion above)
        if (currentState == MovementState.Aiming)
        {
            return; // Keep the Aiming state until cancelled by input
        }

        // --- Standard State Transitions (Walking, Sprinting, Crouching, Prone) ---
        // These transitions should not occur if already aiming or diving.

        // Sprinting Check (Overrides Walking if conditions met)
        // Can only sprint if not already crouching or prone.
        if (sprintInputPressed && isGrounded && currentStamina > 0 && moveInput.magnitude > 0.1f &&
            currentState != MovementState.Crouching && currentState != MovementState.Prone)
        {
            currentState = MovementState.Sprinting;
        }
        // Revert from Sprinting to Walking
        else if (currentState == MovementState.Sprinting &&
                 (!sprintInputPressed || currentStamina <= 0 || moveInput.magnitude < 0.1f))
        {
            currentState = MovementState.Walking;
        }
        // Default to Walking if not in a more specific state (Crouch, Prone, Sprint)
        // This ensures that if a state like Crouch or Prone is exited (e.g. via OnCrouch/OnProne callbacks),
        // and no other condition (like sprint) is met, it falls back to Walking.
        else if (currentState != MovementState.Crouching &&
                 currentState != MovementState.Prone &&
                 currentState != MovementState.Sprinting) // And not Aiming/Diving (handled above)
        {
            currentState = MovementState.Walking;
        }
        // Note: Crouch and Prone state entries are primarily handled by their OnCrouch/OnProne input callbacks.
        // Exiting Crouch/Prone to Walking is also handled there if CanStandUp allows.
    }


    // --- Movement Logic ---
    void HandleMovement()
    {
        float currentSpeedMultiplier = 1f;
        switch (currentState)
        {
            case MovementState.Walking:
            case MovementState.Aiming: // Use walking speed while aiming (can be adjusted)
                currentSpeedMultiplier = 1f;
                break;
            case MovementState.Sprinting:
                currentSpeedMultiplier = sprintMultiplier;
                break;
            case MovementState.Crouching:
                currentSpeedMultiplier = crouchMultiplier;
                break;
            case MovementState.Prone: // *** ADD PRONE SPEED HANDLING ***
                currentSpeedMultiplier = proneMultiplier;
                break;
            // Note: Diving state has its own movement logic below
        }

        if (currentState == MovementState.Diving)
        {
            // Apply forward dive force.
            // The direction of the dive should ideally be based on player's orientation or input at the moment of dive initiation.
            // For simplicity, using current transform.forward. This could be refined to cache the direction at dive start.
            Vector3 diveDirection = transform.forward; // Default to player's forward

            // If there was movement input when dive was initiated, use that direction
            // (moveInput is read each frame, so this uses current input; for dive, might be better to cache at OnDive)
            if (moveInput.magnitude > 0.1f)
            {
                // Convert 2D input to world direction relative to player's orientation
                Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
                diveDirection = transform.TransformDirection(inputDirection);
            }
            
            characterController.Move(diveDirection * diveForce * Time.deltaTime);
        }
        else
        {
            // Standard movement for other states
            float targetSpeed = walkSpeed * currentSpeedMultiplier;
            Vector3 moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized; // Normalize for consistent speed
            characterController.Move(moveDirection * targetSpeed * Time.deltaTime);
        }
    }

    // --- Height Transition (Generalized for Crouch and Prone) ---
    void HandleCrouchTransition() // Renaming to HandleHeightTransition might be clearer later
    {
        // if (currentState == MovementState.Aiming) return; // Optional: prevent height change while aiming

        float currentTransitionSpeed = crouchTransitionSpeed; // Default to crouch speed

        if (currentState == MovementState.Crouching)
        {
            targetHeight = crouchingHeight;
            targetCenter.y = crouchingCenterY;
        }
        else if (currentState == MovementState.Prone)
        {
            targetHeight = proneHeight;
            targetCenter.y = proneCenterY;
            currentTransitionSpeed = proneTransitionSpeed; // Use prone specific speed
        }
        else // Standing or other states like Sprinting, Walking
        {
            targetHeight = standingHeight;
            targetCenter.y = standingCenterY;
        }

        float newHeight = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * currentTransitionSpeed);
        Vector3 newCenter = Vector3.Lerp(characterController.center, targetCenter, Time.deltaTime * currentTransitionSpeed);

        characterController.height = newHeight;
        characterController.center = newCenter;
    }

    // --- Check if player can change to a taller stance ---
    bool CanStandUp(float targetHeight, float targetCenterY)
    {
        // Current bottom of the capsule
        Vector3 currentBottomSphereCenter = transform.position + characterController.center - Vector3.up * (characterController.height / 2f - characterController.radius);

        // Target top of the capsule
        Vector3 targetTopSphereCenter = transform.position + new Vector3(0, targetCenterY, 0) + Vector3.up * (targetHeight / 2f - characterController.radius);

        float checkRadius = characterController.radius - 0.05f; // Slightly smaller radius for check
        float checkDistance = Vector3.Distance(currentBottomSphereCenter, targetTopSphereCenter);

        // Check upwards from current bottom to target top configuration
        // We are checking if a capsule of targetHeight can fit.
        // The capsule cast here is from the *current* bottom sphere to the *target* top sphere.
        // This effectively checks the space required for the *target* capsule height.
        return !Physics.CapsuleCast(
            currentBottomSphereCenter,
            targetTopSphereCenter,
            checkRadius,
            Vector3.up, // Direction of cast is not super relevant here as we check the volume
            0.01f, // Minimal distance, effectively checking the space itself
            ~LayerMask.GetMask("Player"),
            QueryTriggerInteraction.Ignore
        );
    }

    // --- Check if player can go prone (simplified) ---
    bool CanGoProne()
    {
        // This is a very basic check. A more robust solution would involve a capsule cast
        // downwards and forwards to ensure enough space, especially if going prone involves forward motion.
        // For now, just check if there's space directly below for the prone height.
        return CanStandUp(proneHeight, proneCenterY); // Re-using CanStandUp logic, but checking for prone dimensions.
                                                      // This isn't perfectly accurate for "going down" but checks if the prone volume is clear.
                                                      // A better check would be a box cast or multiple raycasts in the prone volume.
    }


    // --- Stamina Management ---
    void HandleStamina()
    {
        bool isConsumingStamina = false;

        if (currentState == MovementState.Sprinting && moveInput.magnitude > 0.1f)
        {
            currentStamina -= sprintStaminaCost * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            timeSinceLastStaminaUse = 0f;
            isConsumingStamina = true;
        }

        if (!isConsumingStamina && currentStamina < maxStamina)
        {
            timeSinceLastStaminaUse += Time.deltaTime;
            if (timeSinceLastStaminaUse >= staminaRegenDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            }
        }
        // UpdateStaminaUI(currentStamina / maxStamina);
    }

    // --- Gravity Application ---
    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    // --- Ground Check ---
    void CheckGrounded()
    {
        Vector3 spherePosition = transform.position + characterController.center;
        spherePosition.y = transform.position.y + characterController.center.y - characterController.height / 2f + characterController.radius - groundCheckOffset;

        isGrounded = Physics.CheckSphere(spherePosition, characterController.radius, groundMask, QueryTriggerInteraction.Ignore);

#if UNITY_EDITOR
        debugSphereColor = isGrounded ? Color.green : Color.red;
        debugSpherePos = spherePosition;
        debugSphereRadius = characterController.radius;
#endif
    }

    // --- Animator ---
    void UpdateAnimator()
    {
        if (animator == null) return;

        // Calculate normalized speed for the animator
        // Use moveInput for responsiveness, but consider characterController.velocity for actual speed if needed
        float normalizedSpeed = moveInput.magnitude;
        if (currentState == MovementState.Sprinting)
        {
            normalizedSpeed *= sprintMultiplier; // Give a higher value for sprint animations if blend tree expects it
        }
        else if (currentState == MovementState.Crouching)
        {
            normalizedSpeed *= crouchMultiplier;
        }
        // Ensure it doesn't exceed 1 if your blend tree is setup for 0-1 for walk/run and a separate trigger/bool for sprint
        // For a simple blend tree:
        // animator.SetFloat("Speed", characterController.velocity.magnitude / (walkSpeed * sprintMultiplier)); // Max speed possible

        // More direct approach based on input and state:
        animator.SetFloat("HorizontalInput", moveInput.x);
        animator.SetFloat("VerticalInput", moveInput.y);
        animator.SetFloat("Speed", normalizedSpeed); // This represents the *intended* speed based on input

        animator.SetBool("IsSprinting", currentState == MovementState.Sprinting);
        animator.SetBool("IsCrouching", currentState == MovementState.Crouching);
        animator.SetBool("IsProne", currentState == MovementState.Prone); // *** ADD PRONE ANIMATOR PARAM ***
        animator.SetBool("IsDiving", currentState == MovementState.Diving); // *** ADD DIVING ANIMATOR PARAM ***
        animator.SetBool("IsAiming", currentState == MovementState.Aiming);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VelocityY", velocity.y); // For jump/fall animations
    }

    // --- UI ---
    // void UpdateStaminaUI(float normalizedStamina) { ... }

    // --- Gizmos ---
#if UNITY_EDITOR
    private Color debugSphereColor = Color.red;
    private Vector3 debugSpherePos;
    private float debugSphereRadius;
    void OnDrawGizmosSelected()
    {
        if (characterController != null)
        {
            Gizmos.color = debugSphereColor;
            Gizmos.DrawWireSphere(debugSpherePos, debugSphereRadius);

            // Draw Stand Up Check Capsule (visualizing the target standing height)
            if (currentState == MovementState.Crouching || currentState == MovementState.Prone)
            {
                float checkTargetHeight = standingHeight;
                float checkTargetCenterY = standingCenterY;
                if (currentState == MovementState.Prone) // If prone, first check for crouch space
                {
                    checkTargetHeight = crouchingHeight;
                    checkTargetCenterY = crouchingCenterY;
                }

                Vector3 currentBottomSphereCenter = transform.position + characterController.center - Vector3.up * (characterController.height / 2f - characterController.radius);
                Vector3 targetTopSphereCenter = transform.position + new Vector3(0, checkTargetCenterY, 0) + Vector3.up * (checkTargetHeight / 2f - characterController.radius);

                Gizmos.color = Color.yellow;
                // Gizmos.DrawWireCapsule(transform.position + new Vector3(0, checkTargetCenterY, 0) , checkTargetHeight / 2f, characterController.radius); // Unity 2020.2+
                // Manual capsule Gizmo for older versions or clarity
                Gizmos.DrawWireSphere(currentBottomSphereCenter, characterController.radius - 0.05f);
                Gizmos.DrawWireSphere(targetTopSphereCenter, characterController.radius - 0.05f);

            }
        }
    }
#endif
}
