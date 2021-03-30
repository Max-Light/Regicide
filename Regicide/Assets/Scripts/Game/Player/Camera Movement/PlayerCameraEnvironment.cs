
using UnityEngine;

namespace Regicide.Game.Player
{
    public class PlayerCameraEnvironment : MonoBehaviour
    {
        public static PlayerCameraEnvironment Singleton { get; private set; } = null;

        [SerializeField] private PolygonCollider2D _virtualCameraConfinerCollider = null;

        public PolygonCollider2D VirtualCameraConfinerCollider { get => _virtualCameraConfinerCollider; }

        private void OnValidate()
        {
            _virtualCameraConfinerCollider = GetComponent<PolygonCollider2D>();
        }

        private void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
                CreateCameraColliderBounds();
            }
            else
            {
                Debug.Log("Multiple player camera environments detected! Destroying superfluous environment...");
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (Singleton == this)
            {
                Singleton = null;
            }
        }

        private void CreateCameraColliderBounds()
        {
            EdgeCollider2D cameraBounds = gameObject.AddComponent(typeof(EdgeCollider2D)) as EdgeCollider2D;
            Vector2[] cameraBoundPoints = new Vector2[_virtualCameraConfinerCollider.points.Length + 1];
            for (int pointIndex = 0; pointIndex < _virtualCameraConfinerCollider.points.Length; pointIndex++)
            {
                cameraBoundPoints[pointIndex] = _virtualCameraConfinerCollider.points[pointIndex];
            }
            cameraBoundPoints[cameraBoundPoints.Length - 1] = _virtualCameraConfinerCollider.points[0];
            cameraBounds.points = cameraBoundPoints;
        }
    }
}