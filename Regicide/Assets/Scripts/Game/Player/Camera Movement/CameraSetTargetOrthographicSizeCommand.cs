
namespace Regicide.Game.Player
{
    public class CameraSetTargetOrthographicSizeCommand : ICommand
    {
        private PlayerCameraMovement playerCameraMovement = null;
        private float scrollDelta = 0;
        private float zoomIncrement = 0;

        public CameraSetTargetOrthographicSizeCommand(PlayerCameraMovement playerCameraMovement, float scrollDelta, float zoomIncrement)
        {
            this.playerCameraMovement = playerCameraMovement;
            this.scrollDelta = scrollDelta;
            this.zoomIncrement = zoomIncrement;
        }

        public void Execute()
        {
            if (scrollDelta != 0)
            {
                playerCameraMovement.TargetOrthographicSize += zoomIncrement * -scrollDelta;
            }
        }
    }
}