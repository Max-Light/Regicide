
using UnityEngine;

namespace Regicide.Game.Player
{
    public class CameraMovementCommand : ICommand
    {
        private Rigidbody _playerRigidbody = null;
        private Vector3 _moveDirection = Vector3.zero;
        float _moveSpeed = 0;

        public CameraMovementCommand(Rigidbody playerRigidbody, Vector2 moveDirection, float moveSpeed) 
        {
            _playerRigidbody = playerRigidbody;
            _moveDirection = new Vector3(moveDirection.x, 0, moveDirection.y);
            _moveSpeed = moveSpeed * 100;
        }

        public void Execute()
        {
            _playerRigidbody.velocity = _moveDirection * _moveSpeed * Time.fixedDeltaTime;
        }
    }
}