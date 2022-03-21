
using UnityEngine;

namespace Regicide.Game.Player
{
    [System.Serializable]
    public class DirectPositionCameraMovementCommand : IPositionalCameraMovementCommand
    {
        [Header("Camera Directional Movement")]
        [SerializeField] [Min(0)] private float _positionalSpeed = 10;
        private Vector2 _moveDirection = Vector2.zero;

        public float PositionalSpeed { get => _positionalSpeed; set => _positionalSpeed = Mathf.Clamp(value, 0, float.MaxValue); }

        public void SetDirection(Vector2 direction)
        {
            _moveDirection = direction;
        }

        public void UpdateCameraPosition(PlayerCameraMovementController cameraControl)
        {
            Vector2 delatDistance = _positionalSpeed * _moveDirection * Time.deltaTime;
            cameraControl.TargetTransform.position += Quaternion.AngleAxis(cameraControl.TargetTransform.eulerAngles.y, Vector3.up) * new Vector3(delatDistance.x, 0, delatDistance.y);
        }
    }
}