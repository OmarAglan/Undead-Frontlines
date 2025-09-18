using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace UndeadFrontlines.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float crouchSpeed = 2.5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float rotationSmoothTime = 0.1f;

        [Header("Camera Settings")]
        [SerializeField] private Transform cameraFollowPoint;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float minCameraAngle = -30f;
        [SerializeField] private float maxCameraAngle = 60f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundMask;

        [Header("Network")]
        [SyncVar] private bool isCrouching;
        [SyncVar] private bool isSprinting;
        [SyncVar] private Vector3 networkPosition;
        [SyncVar] private Quaternion networkRotation;

        // Components
        private CharacterController controller;
        private Camera playerCamera;
        private Transform cameraTransform;

        // Movement state
        private Vector3 velocity;
        private bool isGrounded;
        private float turnSmoothVelocity;
        private float cameraPitch;

        // Input
        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool jumpPressed;
        private bool sprintHeld;
        private bool crouchHeld;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
            {
                // Disable camera rig for non-owners
                if (cameraFollowPoint != null)
                    cameraFollowPoint.gameObject.SetActive(false);
                return;
            }

            SetupCamera();
            Cursor.lockState = CursorLockMode.Locked;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            controller = GetComponent<CharacterController>();
            if (controller == null)
                controller = gameObject.AddComponent<CharacterController>();
        }

        private void SetupCamera()
        {
            // Create or find camera
            GameObject cameraObj = GameObject.FindGameObjectWithTag("PlayerCamera");
            if (cameraObj == null)
            {
                cameraObj = new GameObject("PlayerCamera");
                cameraObj.tag = "PlayerCamera";
                playerCamera = cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<AudioListener>();
            }
            else
            {
                playerCamera = cameraObj.GetComponent<Camera>();
            }

            cameraTransform = playerCamera.transform;
            cameraTransform.SetParent(cameraFollowPoint);
            cameraTransform.localPosition = new Vector3(0, 0, -5f);
            cameraTransform.localRotation = Quaternion.identity;
        }

        private void Update()
        {
            if (!IsOwner) return;

            GatherInput();
            HandleCameraRotation();
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                ServerMove();
            }

            if (!IsOwner && IsClient)
            {
                // Interpolate non-owner positions
                InterpolatePosition();
            }
        }

        private void GatherInput()
        {
            moveInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

            lookInput = new Vector2(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y")
            );

            jumpPressed = Input.GetKeyDown(KeyCode.Space);
            sprintHeld = Input.GetKey(KeyCode.LeftShift);
            crouchHeld = Input.GetKey(KeyCode.LeftControl);

            // Send input to server
            if (IsClient)
            {
                SendMovementInput(moveInput, jumpPressed, sprintHeld, crouchHeld);
            }
        }

        [ServerRpc]
        private void SendMovementInput(Vector2 move, bool jump, bool sprint, bool crouch)
        {
            moveInput = move;
            jumpPressed = jump;
            isSprinting = sprint;
            isCrouching = crouch;
        }

        private void ServerMove()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // Calculate move direction
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

            // Apply appropriate speed
            float currentSpeed = walkSpeed;
            if (isCrouching)
                currentSpeed = crouchSpeed;
            else if (isSprinting)
                currentSpeed = sprintSpeed;

            controller.Move(move * currentSpeed * Time.deltaTime);

            // Jump
            if (jumpPressed && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            // Update network position
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }

        private void HandleCameraRotation()
        {
            // Horizontal rotation (Y axis)
            transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

            // Vertical rotation (X axis) for camera
            cameraPitch -= lookInput.y * mouseSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, minCameraAngle, maxCameraAngle);

            cameraFollowPoint.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }

        private void InterpolatePosition()
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
        }
    }
}