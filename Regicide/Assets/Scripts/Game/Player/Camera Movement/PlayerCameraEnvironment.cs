
using UnityEngine;

namespace Regicide.Game.Player
{
    [RequireComponent(typeof(Collider))]
    public class PlayerCameraEnvironment : MonoBehaviour
    {
        private static Collider _cameraEnvironmentConfiner = null;

        public static Collider CameraEnvironmentConfiner { get => _cameraEnvironmentConfiner; }

        private void Awake()
        {
            if (_cameraEnvironmentConfiner == null)
            {
                _cameraEnvironmentConfiner = GetComponent<Collider>();
            }
        }

        private void OnDestroy()
        {
            _cameraEnvironmentConfiner = null;
        }
    }
}