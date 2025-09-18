using UnityEngine;

namespace UndeadFrontlines.Player
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0, 2, -5);

        [Header("Camera Settings")]
        [SerializeField] private float distance = 5f;
        [SerializeField] private float minDistance = 2f;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private float scrollSpeed = 2f;
        [SerializeField] private float smoothTime = 0.1f;

        [Header("Collision")]
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float collisionRadius = 0.3f;

        private Vector3 currentVelocity;
        private float currentDistance;

        private void Start()
        {
            currentDistance = distance;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Handle scroll wheel zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            currentDistance -= scroll * scrollSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

            // Calculate desired position
            Vector3 desiredPosition = target.position + target.rotation * offset.normalized * currentDistance;

            // Check for collision
            RaycastHit hit;
            if (Physics.SphereCast(target.position, collisionRadius,
                desiredPosition - target.position, out hit,
                currentDistance, collisionMask))
            {
                currentDistance = hit.distance - collisionRadius;
            }

            // Smooth camera movement
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition,
                ref currentVelocity, smoothTime);

            // Always look at target
            transform.LookAt(target.position + Vector3.up * 1.5f);
        }
    }
}