using UnityEngine.InputSystem;
using UnityEngine;

namespace Regicide.Game.Player
{
    public class DragPositionCameraMovementCommand : IPositionalCameraMovementCommand
    {
        private Vector2 _originPoint;

        public void SetOriginDragPoint(Vector2 dragPoint)
        {
            _originPoint = dragPoint;
        }

        public void UpdateCameraPosition(PlayerCameraMovementControl cameraControl)
        {
            throw new System.NotImplementedException();
        }
    }
}