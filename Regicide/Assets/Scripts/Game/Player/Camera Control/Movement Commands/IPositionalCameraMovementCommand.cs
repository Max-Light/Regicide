using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Player
{
    public interface IPositionalCameraMovementCommand 
    {
        public void UpdateCameraPosition(PlayerCameraMovementControl cameraControl);
    }
}