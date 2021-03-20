
using Mirror;
using Regicide.Game.PlayerTurnSystem;
using UnityEngine;

namespace Regicide.Game.GameStates
{
    public class SetupState : ServerGameState
    {
        PlayerCountyAssignmentTurnCycler activeTurnCycler = null;

        public override void OnStateEnable(ServerGameStateCycler cycler)
        {
            base.OnStateEnable(cycler);
            Debug.Log("Setting up game");
            activeTurnCycler = cycler.InstantiateCountyAssignmentTurnCycler();
            NetworkServer.Spawn(activeTurnCycler.gameObject);
        }

        public override void OnStateDisable(ServerGameStateCycler cycler)
        {
            base.OnStateDisable(cycler);
            NetworkServer.Destroy(activeTurnCycler.gameObject);
            activeTurnCycler = null;
        }
    }
}