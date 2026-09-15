using UnityEngine;
using TreasureDivers.Player;

namespace TreasureDivers.Interaction
{
    [DisallowMultipleComponent]
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [Header("Ray")]
        [SerializeField] private Transform rayOrigin;
        [SerializeField, Min(0.1f)] private float interactionRange = 3f;
        [SerializeField] private LayerMask interactionLayers = ~0;
        [SerializeField] private bool drawDebugRay = true;

        private DiverInputReader inputReader;
        private IInteractable currentTarget;

        public IInteractable CurrentTarget => currentTarget;

        private void Awake()
        {
            inputReader = GetComponent<DiverInputReader>();
        }

        private void Update()
        {
            UpdateTarget();

            if (currentTarget != null && inputReader != null && inputReader.InteractPressedThisFrame)
            {
                currentTarget.Interact(this);
            }
        }

        public void SetRayOrigin(Transform origin)
        {
            rayOrigin = origin;
        }

        private void UpdateTarget()
        {
            currentTarget = null;

            Transform origin = rayOrigin != null ? rayOrigin : transform;
            Vector3 start = origin.position;
            Vector3 direction = origin.forward;

            if (drawDebugRay)
            {
                Debug.DrawRay(start, direction * interactionRange, Color.yellow);
            }

            if (!Physics.Raycast(start, direction, out RaycastHit hit, interactionRange, interactionLayers, QueryTriggerInteraction.Ignore))
            {
                return;
            }

            currentTarget = FindInteractable(hit.collider);
        }

        private static IInteractable FindInteractable(Collider source)
        {
            MonoBehaviour[] behaviours = source.GetComponentsInParent<MonoBehaviour>();

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IInteractable interactable)
                {
                    return interactable;
                }
            }

            return null;
        }
    }
}
