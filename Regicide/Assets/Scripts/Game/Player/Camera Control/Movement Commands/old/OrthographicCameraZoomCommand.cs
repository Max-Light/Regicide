using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Regicide.Game.Player
{
    public class OrthographicCameraZoomCommand : ICommand
    {
        private PlayerCameraMovement _playerCameraMovement = null;

        public OrthographicCameraZoomCommand(PlayerCameraMovement playerCameraMovement)
        {
            _playerCameraMovement = playerCameraMovement;
        }

        public void Execute()
        {
            LensSettings virtualCameraLens = _playerCameraMovement.CinemachineVirtualCamera.m_Lens;
            float newOrthographicSize = Mathf.Lerp(virtualCameraLens.OrthographicSize, _playerCameraMovement.TargetOrthographicSize, Time.fixedDeltaTime * _playerCameraMovement.CameraZoomingSpeed);
            _playerCameraMovement.CinemachineVirtualCamera.m_Lens.OrthographicSize = newOrthographicSize;
            if (Pointer.current != null)
            {
                float orthographicSizeDelta = virtualCameraLens.OrthographicSize - newOrthographicSize;
                Vector2 pointerResolutionFromCameraOrigin = (Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue()) - _playerCameraMovement.transform.position);
                Vector2 resolutionScaleFromCameraOrigin = new Vector2(Mathf.Clamp(pointerResolutionFromCameraOrigin.x / (virtualCameraLens.OrthographicSize * virtualCameraLens.Aspect), -1, 1), Mathf.Clamp(pointerResolutionFromCameraOrigin.y / virtualCameraLens.OrthographicSize, -1, 1));
                _playerCameraMovement.transform.position += new Vector3(resolutionScaleFromCameraOrigin.x * (orthographicSizeDelta * virtualCameraLens.Aspect), 0, resolutionScaleFromCameraOrigin.y * orthographicSizeDelta);
            }
        }
    }
}