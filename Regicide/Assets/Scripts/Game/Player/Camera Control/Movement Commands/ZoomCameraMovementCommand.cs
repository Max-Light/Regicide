using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Player
{
    [System.Serializable]
    public class ZoomCameraMovementCommand : IZoomCameraMovementCommand
    {
        [Header("Camera Zoom Movement")]
        [SerializeField] [Min(0)] private float _zoomIncrement = 5;
        [SerializeField] [Min(0)] private float _zoomSpeed = 10;

        public float ZoomIncrement { get => _zoomIncrement; set => _zoomIncrement = Mathf.Clamp(value, 0, float.MaxValue); }
        public float ZoomSpeed { get => _zoomSpeed; set => _zoomSpeed = Mathf.Clamp(value, 0, float.MaxValue); }

        public void UpdateCameraZoom(PlayerCameraMovementControl cameraControl)
        {
            
        }
    }
}