using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Player
{
    public interface IZoomCameraMovementCommand
    {
        public void UpdateCameraZoom(PlayerCameraMovementController cameraControl);
    }
}