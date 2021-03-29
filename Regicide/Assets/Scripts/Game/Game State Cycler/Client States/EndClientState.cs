
namespace Regicide.Game.GameStates
{
    public class EndClientState : GameClientState
    {
        public EndClientState()
        {
            GameStateId = 4;
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