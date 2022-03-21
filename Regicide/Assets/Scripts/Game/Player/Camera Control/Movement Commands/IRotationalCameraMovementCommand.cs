using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Player
{
    public interface IRotationalCameraMovementCommand 
    {
        public void UpdateCameraRotation(PlayerCameraMovementController cameraControl);
    }
}