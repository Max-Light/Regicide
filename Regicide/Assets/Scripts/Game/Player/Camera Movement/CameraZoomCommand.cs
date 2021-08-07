using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Regicide.Game.Player
{
    public class CameraZoomCommand : ICommand
    {
        Rigidbody _playerRigidbody = null;
        CinemachineVirtualCamera _virtualCamera = null;
        BoxCollider _cameraCollider = null;
        private float _zoomSpeed = 0;
        private float _targetOrthographicSize = 0;

        public CameraZoomCommand(Rigidbody playerRigidbody, CinemachineVirtualCamera virtualCamera, BoxCollider cameraCollider, float targetOrthographicSize, float zoomSpeed)
        {
            this._playerRigidbody = playerRigidbody;
            this._virtualCamera = virtualCamera;
            this._cameraCollider = cameraCollider;
            this._targetOrthographicSize = targetOrthographicSize;
            this._zoomSpeed = zoomSpeed;
        }

        public void Execute()
        {
            LensSettings virtualCameraLens = _virtualCamera.m_Lens;
            float newOrthographicSize = Mathf.Lerp(virtualCameraLens.OrthographicSize, _targetOrthographicSize, Time.fixedDeltaTime * _zoomSpeed);
            _cameraCollider.size = new Vector3(virtualCameraLens.OrthographicSize * _virtualCamera.m_Lens.Aspect * 2, virtualCameraLens.OrthographicSize * 2, 1);
            _virtualCamera.m_Lens.OrthographicSize = newOrthographicSize;
            if (Pointer.current != null)
            {
                float orthographicSizeDelta = virtualCameraLens.OrthographicSize - newOrthographicSize;
                Vector2 pointerResolutionFromCameraOrigin = (Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue()) - _playerRigidbody.transform.position);
                Vector2 resolutionScaleFromCameraOrigin = new Vector2(Mathf.Clamp(pointerResolutionFromCameraOrigin.x / (virtualCameraLens.OrthographicSize * virtualCameraLens.Aspect), -1, 1), Mathf.Clamp(pointerResolutionFromCameraOrigin.y / virtualCameraLens.OrthographicSize, -1, 1));
                _playerRigidbody.transform.position += new Vector3(resolutionScaleFromCameraOrigin.x * (orthographicSizeDelta * virtualCameraLens.Aspect), 0, resolutionScaleFromCameraOrigin.y * orthographicSizeDelta);
            }
        }
    }
}