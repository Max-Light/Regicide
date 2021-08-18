using UnityEngine;
using UnityEngine.InputSystem;

namespace Regicide.Game.Player
{
    public class CameraDragMovementCommand : ICommand
    {
        private PlayerCameraMovement _playerCameraMovement = null;

        public CameraDragMovementCommand(PlayerCameraMovement playerCameraMovement)
        {
            _playerCameraMovement = playerCameraMovement;
        }

        public void Execute()
        {
            if (Pointer.current != null)
            {
                Vector2 screenWorldUnitResolution = _playerCameraMovement.CameraWorldUnitResolution;
                Vector3 cameraPosition = _playerCameraMovement.transform.position;
                Vector2 viewportOffset = Camera.main.ScreenToViewportPoint(_playerCameraMovement.DragPointerOriginPosition - Pointer.current.position.ReadValue());
                Vector2 movement = viewportOffset * screenWorldUnitResolution;

                Vector3 updatedCameraPosition = new Vector3(movement.x + _playerCameraMovement.DragCameraOriginalWorldPosition.x, cameraPosition.y, movement.y + _playerCameraMovement.DragCameraOriginalWorldPosition.y);
                _playerCameraMovement.transform.position = updatedCameraPosition;
            }
        }
    }
}