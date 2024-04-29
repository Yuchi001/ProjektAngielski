using UnityEngine;

namespace Managers
{
    public class MainCamera : MonoBehaviour
    {
        private Transform _player;

        public void Setup(GameObject player)
        {
            _player = player.transform;
        }

        private void Update()
        {
            if (_player == null) return;

            transform.position = new Vector3(_player.position.x, _player.position.y, -10);
        }
    }
}