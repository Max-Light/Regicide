
namespace Regicide.Game.GameStates
{
    public class EndServerState : GameServerState
    {
        public EndServerState()
        {
            ClientStateId = new EndClientState().GameStateId;
        }

        public override void OnStateEnable(ServerGameStateCycler cycler)
        {
            base.OnStateEnable(cycler);
        }

        public override void OnStateDisable(ServerGameStateCycler cycler)
        {
            base.OnStateDisable(cycler);
        }
    }
}