using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Player
{
    [System.Serializable]
    public class RotationalCameraMovement : IRotationalCameraMovementCommand
    {
        [Header("Camera Rotational Movement")]
        [SerializeField] [Min(0)] private float _rotationalSpeed = 15;

        public float RotationalSpeed { get => _rotationalSpeed; set => _rotationalSpeed = Mathf.Clamp(value, 0, float.MaxValue); }

        public void UpdateCameraRotation(PlayerCameraMovementControl cameraControl)
        {
            
        }
    }
}