
using UnityEngine;

namespace Regicide.Game.Player
{
    public class CameraMovementCommand
    {
        public CameraMovementCommand() { }

        public void Execute(Rigidbody2D playerRigidbody, Vector2 moveVector, float speed)
        {
            playerRigidbody.velocity = moveVector * speed * Time.fixedDeltaTime;
        }
    }
}