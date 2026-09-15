using TreasureDivers.World;
using UnityEngine;

namespace TreasureDivers.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(DiverInputReader))]
    [RequireComponent(typeof(WaterSensor))]
    public sealed class FirstPersonDiverMotor : MonoBehaviour
    {
        [Header("View")]
        [SerializeField] private Transform viewTransform;
        [SerializeField, Range(0.01f, 1f)] private float mouseSensitivity = 0.12f;
        [SerializeField, Range(45f, 89f)] private float maxPitch = 84f;

        [Header("Walking")]
        [SerializeField, Min(0f)] private float walkSpeed = 4.5f;
        [SerializeField, Min(0f)] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private float groundedStickVelocity = -2f;

        [Header("Ground Detection")]
        [SerializeField, Min(0.01f)] private float groundCheckRadius = 0.28f;
        [SerializeField, Min(0.01f)] private float groundCheckDistance = 0.18f;
        [SerializeField] private LayerMask groundLayers = ~0;

        [Header("Swimming")]
        [SerializeField, Min(0f)] private float swimSpeed = 3.25f;
        [SerializeField, Min(0f)] private float swimVerticalSpeed = 2.75f;
        [SerializeField, Min(0.01f)] private float swimTransitionSpeed = 4f;

        private CharacterController controller;
        private DiverInputReader inputReader;
        private WaterSensor waterSensor;
        private float pitch;
        private float verticalVelocity;
        private float swimBlend;
        private bool isGrounded;

        public bool IsGrounded => isGrounded;
        public bool IsSwimming => swimBlend > 0.5f;
        public float SwimBlend => swimBlend;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            inputReader = GetComponent<DiverInputReader>();
            waterSensor = GetComponent<WaterSensor>();

            if (viewTransform == null && Camera.main != null)
            {
                viewTransform = Camera.main.transform;
            }
        }

        private void Start()
        {
            if (viewTransform != null)
            {
                Vector3 euler = viewTransform.localEulerAngles;
                pitch = NormalizePitch(euler.x);
            }
        }

        private void Update()
        {
            UpdateLook();
            UpdateMovement();
        }

        public void SetViewTransform(Transform view)
        {
            viewTransform = view;
        }

        private void UpdateLook()
        {
            if (viewTransform == null || Cursor.lockState != CursorLockMode.Locked)
            {
                return;
            }

            Vector2 look = inputReader.LookDelta;
            float yaw = look.x * mouseSensitivity;
            pitch = Mathf.Clamp(pitch - look.y * mouseSensitivity, -maxPitch, maxPitch);

            transform.Rotate(Vector3.up, yaw, Space.World);
            viewTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void UpdateMovement()
        {
            isGrounded = CheckGrounded();
            bool shouldSwim = waterSensor != null && waterSensor.IsInWater;
            swimBlend = Mathf.MoveTowards(swimBlend, shouldSwim ? 1f : 0f, swimTransitionSpeed * Time.deltaTime);

            Vector2 moveInput = Vector2.ClampMagnitude(inputReader.Move, 1f);
            Vector3 localMove = new Vector3(moveInput.x, 0f, moveInput.y);
            Vector3 horizontalDirection = transform.TransformDirection(localMove);
            float horizontalSpeed = Mathf.Lerp(walkSpeed, swimSpeed, swimBlend);

            UpdateVerticalVelocity(shouldSwim);

            Vector3 velocity = horizontalDirection * horizontalSpeed;
            velocity.y = verticalVelocity;

            controller.Move(velocity * Time.deltaTime);
            isGrounded = CheckGrounded();
        }

        private void UpdateVerticalVelocity(bool shouldSwim)
        {
            if (swimBlend > 0.01f)
            {
                float verticalInput = 0f;

                if (inputReader.AscendHeld)
                {
                    verticalInput += 1f;
                }

                if (inputReader.SwimDownHeld)
                {
                    verticalInput -= 1f;
                }

                float targetSwimVelocity = verticalInput * swimVerticalSpeed;
                verticalVelocity = Mathf.Lerp(verticalVelocity, targetSwimVelocity, swimBlend);
                return;
            }

            if (isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = groundedStickVelocity;
            }

            if (!shouldSwim && isGrounded && inputReader.JumpPressedThisFrame)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            verticalVelocity += gravity * Time.deltaTime;
        }

        private bool CheckGrounded()
        {
            if (controller.isGrounded)
            {
                return true;
            }

            Vector3 origin = transform.TransformPoint(controller.center);
            float castDistance = Mathf.Max(0f, controller.height * 0.5f - controller.radius) + groundCheckDistance;

            return Physics.SphereCast(
                origin,
                groundCheckRadius,
                Vector3.down,
                out _,
                castDistance,
                groundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private static float NormalizePitch(float angle)
        {
            return angle > 180f ? angle - 360f : angle;
        }
    }
}
