
using Mirror;
using Regicide.Game.PlayerTurnSystem;
using UnityEngine;

namespace Regicide.Game.GameStates
{
    public class SetupServerState : GameServerState
    {
        PlayerCountyAssignmentTurnCycler activeTurnCycler = null;

        public SetupServerState()
        {
            ClientStateId = new SetupClientState().GameStateId;
        }

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