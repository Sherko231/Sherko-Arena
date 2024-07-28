using UnityEngine;

namespace Sherko.MonoBehaviours
{
    public class LookAtPlayer : MonoBehaviour
    {
        [SerializeField] private Transform _player;

        private void Update() => transform.LookAt(_player.position);
    }
}

