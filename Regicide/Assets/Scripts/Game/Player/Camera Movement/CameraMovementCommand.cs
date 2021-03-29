
using UnityEngine;

namespace Regicide.Game.Player
{
    public class CameraMovementCommand : ICommand
    {
        private Rigidbody2D playerRigidbody = null;
        private Vector2 moveDirection = Vector2.zero;
        float moveSpeed = 0;

        public CameraMovementCommand(Rigidbody2D playerRigidbody, Vector2 moveDirection, float moveSpeed) 
        {
            this.playerRigidbody = playerRigidbody;
            this.moveDirection = moveDirection;
            this.moveSpeed = moveSpeed;
        }

        public void Execute()
        {
            playerRigidbody.velocity = moveDirection * moveSpeed * Time.fixedDeltaTime;
        }
    }
}