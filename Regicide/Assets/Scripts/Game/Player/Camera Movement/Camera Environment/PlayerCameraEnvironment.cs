
using UnityEngine;

namespace Regicide.Game.Player
{
    [RequireComponent(typeof(Collider))]
    public class PlayerCameraEnvironment : MonoBehaviour
    {
        private static Collider _cameraEnvironmentConfiner = null;

        public static Collider CameraEnvironmentConfiner { get => _cameraEnvironmentConfiner; }

        public static void ConfineTransform(Transform transform, Vector2 resolution)
        {
            if (_cameraEnvironmentConfiner == null)
            {
                Debug.LogError("Cannot confine transform because there is no collider!");
                return;
            }
            resolution /= 2;
            Vector3 position = transform.position;

            float topPoint = resolution.y + position.z;
            float bottomPoint = -resolution.y + position.z;
            float leftPoint = -resolution.x + position.x;
            float rightPoint = resolution.x + position.x;

            float minX = _cameraEnvironmentConfiner.bounds.min.x;
            float maxX = _cameraEnvironmentConfiner.bounds.max.x;
            float minZ = _cameraEnvironmentConfiner.bounds.min.z;
            float maxZ = _cameraEnvironmentConfiner.bounds.max.z;

            if (topPoint > maxZ)
            {
                position = new Vector3(position.x, transform.position.y, maxZ - resolution.y);
            }
            if (bottomPoint < minZ)
            {
                position = new Vector3(position.x, transform.position.y, minZ + resolution.y);
            }
            if (leftPoint < minX)
            {
                position = new Vector3(minX + resolution.x, transform.position.y, position.z);
            }
            if (rightPoint > maxX)
            {
                position = new Vector3(maxX - resolution.x, transform.position.y, position.z);
            }
            position = new Vector3(position.x, Mathf.Clamp(position.y, _cameraEnvironmentConfiner.bounds.min.y, _cameraEnvironmentConfiner.bounds.max.y), position.z);
            transform.position = position;
        }


        private void Awake()
        {
            if (_cameraEnvironmentConfiner == null)
            {
                if (TryGetComponent(out Collider boundingCollider))
                {
                    _cameraEnvironmentConfiner = boundingCollider;
                }
                else
                {
                    Debug.LogWarning("No bounding collider found!");
                }
            }
        }

        private void OnDestroy()
        {
            if (_cameraEnvironmentConfiner == GetComponent<Collider>())
            {
                _cameraEnvironmentConfiner = null;
            }
        }
    }
}