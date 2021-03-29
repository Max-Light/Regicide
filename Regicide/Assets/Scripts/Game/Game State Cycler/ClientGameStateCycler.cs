using Mirror;
using UnityEngine;

namespace Regicide.Game.GameStates
{
    public class ClientGameStateCycler : NetworkBehaviour
    {
        public static ClientGameStateCycler Singleton { get; private set; } = null;

        [SyncVar(hook = nameof(OnGameStateChange))]
        private uint gameStateId = GameClientState.Nil.GameStateId;
        private GameClientState currentGameState = GameClientState.Nil;

        private void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
                GameClientStateFactory.InitializeStateFactory();
            }
            else
            {
                Debug.LogWarning("Multiple client game state cyclers detected! Destroying superfluous cyclers...");
                Destroy(this);
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            SwitchToGameClientState(GameClientStateFactory.GetGameState(gameStateId));
        }

        private void OnDestroy()
        {
            if (this == Singleton)
            {
                Singleton = null;
                GameClientStateFactory.ClearStateFactory();
            }
        }

        private void OnGameStateChange(uint _, uint stateId)
        {
            SwitchToGameClientState(GameClientStateFactory.GetGameState(stateId));
        }

        private void SwitchToGameClientState(GameClientState gameState)
        {
            if (gameState == null)
            {
                gameState = GameClientState.Nil;
            }
            currentGameState.OnStateDisable(this);
            currentGameState = gameState;
            currentGameState.OnStateEnable(this);
        }

        [Server]
        public void SetClientGameState(uint clientStateId)
        {
            gameStateId = clientStateId;
        }
    }
}