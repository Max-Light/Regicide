
using UnityEngine;

namespace Regicide.Game.Player
{
    public class CameraMovementCommand : ICommand
    {
        private Rigidbody2D _playerRigidbody = null;
        private Vector2 _moveDirection = Vector2.zero;
        float _moveSpeed = 0;

        public CameraMovementCommand(Rigidbody2D playerRigidbody, Vector2 moveDirection, float moveSpeed) 
        {
            this._playerRigidbody = playerRigidbody;
            this._moveDirection = moveDirection;
            this._moveSpeed = moveSpeed;
        }

        public void Execute()
        {
            _playerRigidbody.velocity = _moveDirection * _moveSpeed * Time.fixedDeltaTime;
        }
    }
}