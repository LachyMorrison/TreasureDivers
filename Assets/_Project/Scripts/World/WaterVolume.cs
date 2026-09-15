using UnityEngine;

namespace TreasureDivers.World
{
    [DisallowMultipleComponent]
    public sealed class WaterVolume : MonoBehaviour
    {
        [SerializeField] private bool drawGizmo = true;
        [SerializeField] private Color gizmoColor = new Color(0.1f, 0.55f, 1f, 0.2f);

        private void Reset()
        {
            MakeColliderATrigger();
        }

        private void OnValidate()
        {
            MakeColliderATrigger();
        }

        private void MakeColliderATrigger()
        {
            Collider volumeCollider = GetComponent<Collider>();

            if (volumeCollider != null)
            {
                volumeCollider.isTrigger = true;
            }
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmo)
            {
                return;
            }

            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.8f);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
