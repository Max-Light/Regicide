
using UnityEngine;

namespace Regicide.Game.GameStates
{
    public class PlayState : ServerGameState
    {
        public override void OnStateEnable(ServerGameStateCycler cycler)
        {
            base.OnStateEnable(cycler);
            Debug.Log("Game in play");
        }

        public override void OnStateDisable(ServerGameStateCycler cycler)
        {
            base.OnStateDisable(cycler);
        }
    }
}