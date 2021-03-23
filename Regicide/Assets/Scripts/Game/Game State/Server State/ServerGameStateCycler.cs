using Mirror;
using Regicide.Game.PlayerTurnSystem;
using System;
using System.Collections;
using UnityEngine;

namespace Regicide.Game.GameStates
{
    public class ServerGameStateCycler : NetworkBehaviour
    {
        public static ServerGameStateCycler Singleton { get; private set; } = null;
        private ServerGameState gameState = ServerGameState.Nil;

        [SerializeField] private PlayerCountyAssignmentTurnCycler countyAssignmentTurnCyclerPrefab = null;

        private void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Debug.LogWarning("Multiple server game state cyclers detected! Destroying superfluous cyclers...");
                Destroy(this);
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SwitchToGameState(new WaitForPlayersState());
        }

        private void OnDestroy()
        {
            if (this == Singleton)
            {
                Singleton = null;
            }
        }

        [Server]
        public void SwitchToGameState(ServerGameState gameState)
        {
            this.gameState.OnStateDisable(this);
            ServerGameState oldState = this.gameState;
            this.gameState = gameState;
            this.gameState.OnStateEnable(this);

            callback?.Invoke(oldState, gameState);
        }

        [Server]
        public PlayerCountyAssignmentTurnCycler InstantiateCountyAssignmentTurnCycler() => Instantiate(countyAssignmentTurnCyclerPrefab.gameObject).GetComponent<PlayerCountyAssignmentTurnCycler>();

        private Action<ServerGameState, ServerGameState> callback = null;

        public void AddObserver(Action<ServerGameState, ServerGameState> observer)
        {
            callback += observer;
        }

        public void RemoveObserver(Action<ServerGameState, ServerGameState> observer)
        {
            callback -= observer;
        }
    }
}