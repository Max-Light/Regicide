using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Regicide.Game.Player
{
    public class CameraZoomCommand : ICommand
    {
        Rigidbody2D playerRigidbody = null;
        CinemachineVirtualCamera virtualCamera = null;
        BoxCollider2D cameraCollider = null;
        private float zoomSpeed = 0;
        private float targetOrthographicSize = 0;

        public CameraZoomCommand(Rigidbody2D playerRigidbody, CinemachineVirtualCamera virtualCamera, BoxCollider2D cameraCollider, float targetOrthographicSize, float zoomSpeed)
        {
            this.playerRigidbody = playerRigidbody;
            this.virtualCamera = virtualCamera;
            this.cameraCollider = cameraCollider;
            this.targetOrthographicSize = targetOrthographicSize;
            this.zoomSpeed = zoomSpeed;
        }

        public void Execute()
        {
            LensSettings virtualCameraLens = virtualCamera.m_Lens;
            float newOrthographicSize = Mathf.Lerp(virtualCameraLens.OrthographicSize, targetOrthographicSize, Time.fixedDeltaTime * zoomSpeed);
            cameraCollider.size = new Vector2(virtualCameraLens.OrthographicSize * virtualCamera.m_Lens.Aspect * 2, virtualCameraLens.OrthographicSize * 2);
            virtualCamera.m_Lens.OrthographicSize = newOrthographicSize;
            if (Pointer.current != null)
            {
                float orthographicSizeDelta = virtualCameraLens.OrthographicSize - newOrthographicSize;
                Vector2 pointerResolutionFromCameraOrigin = (Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue()) - playerRigidbody.transform.position);
                Vector2 resolutionScaleFromCameraOrigin = new Vector2(Mathf.Clamp(pointerResolutionFromCameraOrigin.x / (virtualCameraLens.OrthographicSize * virtualCameraLens.Aspect), -1, 1), Mathf.Clamp(pointerResolutionFromCameraOrigin.y / virtualCameraLens.OrthographicSize, -1, 1));
                playerRigidbody.transform.position += new Vector3(resolutionScaleFromCameraOrigin.x * (orthographicSizeDelta * virtualCameraLens.Aspect), resolutionScaleFromCameraOrigin.y * orthographicSizeDelta, 0);
            }
        }
    }
}