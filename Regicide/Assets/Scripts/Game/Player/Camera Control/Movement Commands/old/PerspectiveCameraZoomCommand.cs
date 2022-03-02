using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Regicide.Game.Player
{
    public class PerspectiveCameraZoomCommand : ICommand
    {
        private PlayerCameraMovement _playerCameraMovement = null;

        public PerspectiveCameraZoomCommand(PlayerCameraMovement playerCameraMovement)
        {
            _playerCameraMovement = playerCameraMovement;
        }

        public void Execute()
        {
            Vector2 previousCameraWorldResolution = _playerCameraMovement.CameraWorldUnitResolution;

            Vector3 cameraPosition = _playerCameraMovement.transform.position;
            float newCameraHeight = Mathf.Lerp(cameraPosition.y, _playerCameraMovement.TargetCameraHeight, Time.fixedDeltaTime * _playerCameraMovement.CameraZoomingSpeed);
            float deltaHeight = newCameraHeight - cameraPosition.y;

            _playerCameraMovement.transform.position += new Vector3(0, deltaHeight, 0);

            if (Pointer.current != null && !_playerCameraMovement.IsMovingCamera)
            {
                Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector2 currentCameraWorldResolution = _playerCameraMovement.CameraWorldUnitResolution;
                    Vector2 cameraMovementAmount = (previousCameraWorldResolution - currentCameraWorldResolution) / 2;

                    Vector2 pointerOffset = new Vector2(hit.point.x - cameraPosition.x, hit.point.z - cameraPosition.z);
                    Vector2 movementDirectionScale = new Vector2(pointerOffset.x / (previousCameraWorldResolution.x / 2), pointerOffset.y / (previousCameraWorldResolution.y / 2));

                    _playerCameraMovement.transform.position += new Vector3(cameraMovementAmount.x * movementDirectionScale.x, 0, cameraMovementAmount.y * movementDirectionScale.y);
                }
            }
        }
    }
}