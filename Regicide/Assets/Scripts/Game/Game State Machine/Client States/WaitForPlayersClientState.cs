
namespace Regicide.Game.GameStates
{
    public class WaitForPlayersClientState : GameClientState
    {
        public WaitForPlayersClientState()
        {
            GameStateId = 1;
        }

        public override void OnStateEnable(ClientGameStateCycler cycler)
        {
            base.OnStateEnable(cycler);
        }

        public override void OnStateDisable(ClientGameStateCycler cycler)
        {
            base.OnStateDisable(cycler);
        }
    }
}