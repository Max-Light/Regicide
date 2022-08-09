using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Player
{
    [System.Serializable]
    public class RotateCameraMovementCommand : IRotationalCameraMovementCommand
    {
        [Header("Camera Rotational Movement")]
        [SerializeField] [Min(0)] private float _rotationalSpeed = 15;
        private float _rotateDirection = 0;

        public float RotationalSpeed { get => _rotationalSpeed; set => _rotationalSpeed = Mathf.Clamp(value, 0, float.MaxValue); }
        public float RotationalDirection { get => _rotateDirection; set => _rotateDirection = value; }

        public void UpdateCameraRotation(PlayerCameraMovementController cameraControl)
        {
            Ray ray = cameraControl.PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                cameraControl.TargetTransform.RotateAround(hit.point, Vector3.up, _rotationalSpeed * _rotateDirection * Time.deltaTime);
            }
        }
    }
}