using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Regicide.Game.Player
{
    public class CameraZoomCommand
    {
        private float minOrthographicSize = 3;
        private float maxOrthographicSize = 10;

        private float targetOrthographicSize = 0;

        public CameraZoomCommand(float startingOrthographicSize) 
        { 
            targetOrthographicSize = startingOrthographicSize;
        }
        public CameraZoomCommand(float startingOrthographicSize, float minOrthographicSize, float maxOrthographicSize)
        {
            targetOrthographicSize = startingOrthographicSize;
            this.minOrthographicSize = minOrthographicSize;
            this.maxOrthographicSize = maxOrthographicSize;
        }

        public void Execute(CinemachineVirtualCamera virtualCamera, float scrollDelta, float zoomIncrement)
        {
            if (scrollDelta != 0)
            {
                float newOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
                newOrthographicSize += zoomIncrement * -scrollDelta;
                targetOrthographicSize = Mathf.Clamp(newOrthographicSize, minOrthographicSize, maxOrthographicSize);
            }
        }

        public void UpdateOrhtoGrahpicSize(Rigidbody2D playerRigidbody, CinemachineVirtualCamera virtualCamera, BoxCollider2D cameraCollider, float zoomSpeed)
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