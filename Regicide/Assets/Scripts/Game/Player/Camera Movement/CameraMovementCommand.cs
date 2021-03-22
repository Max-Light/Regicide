
using UnityEngine;

namespace Regicide.Game.Player
{
    public class CameraMovementCommand
    {
        public CameraMovementCommand() { }

        public void Execute(Rigidbody2D cameraRigidbody, Vector2 moveVector, float speed)
        {
            cameraRigidbody.velocity = moveVector * speed * Time.fixedDeltaTime;
        }
    }
}