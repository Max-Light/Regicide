
using UnityEngine;

namespace Regicide.Game.Player
{
    public class PlayerCameraEnvironment : MonoBehaviour
    {
        public static PlayerCameraEnvironment Singleton { get; private set; } = null;

        [SerializeField] private PolygonCollider2D virtualCameraConfinerCollider = null;

        private void OnValidate()
        {
            virtualCameraConfinerCollider = GetComponent<PolygonCollider2D>();
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

        public PolygonCollider2D VirtualCameraConfinerCollider { get => virtualCameraConfinerCollider; }

        private void CreateCameraColliderBounds()
        {
            EdgeCollider2D cameraBounds = gameObject.AddComponent(typeof(EdgeCollider2D)) as EdgeCollider2D;
            Vector2[] cameraBoundPoints = new Vector2[virtualCameraConfinerCollider.points.Length + 1];
            for (int pointIndex = 0; pointIndex < virtualCameraConfinerCollider.points.Length; pointIndex++)
            {
                cameraBoundPoints[pointIndex] = virtualCameraConfinerCollider.points[pointIndex];
            }
            cameraBoundPoints[cameraBoundPoints.Length - 1] = virtualCameraConfinerCollider.points[0];
            cameraBounds.points = cameraBoundPoints;
        }
    }
}