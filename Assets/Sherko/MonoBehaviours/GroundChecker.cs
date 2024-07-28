using UnityEngine;

namespace Sherko.MonoBehaviours
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private Vector3 size = new(1, 1, 1);
        [SerializeField] private Color gizmosColor = Color.yellow;
        [SerializeField] private LayerMask groundLayer;

        public bool Value => Physics.CheckBox(transform.position, size / 2, Quaternion.identity, groundLayer);

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawWireCube(transform.position, size);
        }
    }
}

