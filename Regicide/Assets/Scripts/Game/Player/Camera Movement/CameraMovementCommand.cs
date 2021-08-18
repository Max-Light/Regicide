
using UnityEngine;

namespace Regicide.Game.Player
{
    public class CameraMovementCommand : ICommand
    {
        private PlayerCameraMovement _playerCameraMovement = null;

        public CameraMovementCommand(PlayerCameraMovement playerCameraMovement) 
        {
            _playerCameraMovement = playerCameraMovement;
        }

        public void Execute()
        {
            Vector3 direction = new Vector3(_playerCameraMovement.CurrentMoveDirection.x, 0, _playerCameraMovement.CurrentMoveDirection.y);
            _playerCameraMovement.transform.Translate(direction * _playerCameraMovement.CameraMovementSpeed * Time.fixedDeltaTime, Space.World);
        }
    }
}