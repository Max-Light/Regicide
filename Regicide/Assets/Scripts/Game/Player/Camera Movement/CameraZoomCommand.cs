using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Player
{
    public class CameraZoomCommand
    {
        private float minOrthographicSize = 3;
        private float maxOrthographicSize = 10;

        public CameraZoomCommand() { }
        public CameraZoomCommand(float minOrthographicSize, float maxOrthographicSize)
        {
            this.minOrthographicSize = minOrthographicSize;
            this.maxOrthographicSize = maxOrthographicSize;
        }

        public void Execute(CinemachineVirtualCamera virtualCamera, BoxCollider2D cameraCollider, float scrollDelta, float zoomSpeed)
        {
            if (scrollDelta != 0)
            {
                float newOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
                newOrthographicSize += zoomSpeed * scrollDelta * Time.fixedDeltaTime;
                virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(newOrthographicSize, minOrthographicSize, maxOrthographicSize);
                LensSettings virtualCameraLens = virtualCamera.m_Lens;
                cameraCollider.size = new Vector2(virtualCameraLens.OrthographicSize * virtualCameraLens.Aspect * 2, virtualCameraLens.OrthographicSize * 2);
            }
        }
    }
}