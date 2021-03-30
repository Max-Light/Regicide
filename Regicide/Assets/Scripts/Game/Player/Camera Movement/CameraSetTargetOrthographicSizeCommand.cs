
namespace Regicide.Game.Player
{
    public class CameraSetTargetOrthographicSizeCommand : ICommand
    {
        private PlayerCameraMovement _playerCameraMovement = null;
        private float _scrollDelta = 0;
        private float _zoomIncrement = 0;

        public CameraSetTargetOrthographicSizeCommand(PlayerCameraMovement playerCameraMovement, float scrollDelta, float zoomIncrement)
        {
            this._playerCameraMovement = playerCameraMovement;
            this._scrollDelta = scrollDelta;
            this._zoomIncrement = zoomIncrement;
        }

        public void Execute()
        {
            if (_scrollDelta != 0)
            {
                _playerCameraMovement.TargetOrthographicSize += _zoomIncrement * -_scrollDelta;
            }
        }
    }
}