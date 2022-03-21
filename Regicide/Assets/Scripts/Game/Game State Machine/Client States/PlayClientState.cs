
using UnityEngine;

namespace Regicide.Game.GameStates
{
    public class PlayClientState : GameClientState
    {
        public PlayClientState()
        {
            GameStateId = 3;
        }

        public override void OnStateEnable(ClientGameStateCycler cycler)
        {
            base.OnStateEnable(cycler);
            Debug.Log("Client in Play State");
        }

        public override void OnStateDisable(ClientGameStateCycler cycler)
        {
            base.OnStateDisable(cycler);
        }
    }
}