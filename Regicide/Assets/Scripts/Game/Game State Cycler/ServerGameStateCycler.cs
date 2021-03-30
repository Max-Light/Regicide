using Mirror;
using Regicide.Game.PlayerTurnSystem;
using UnityEngine;

namespace Regicide.Game.GameStates
{
    public class ServerGameStateCycler : NetworkBehaviour
    {
        public static ServerGameStateCycler Singleton { get; private set; } = null;
        private GameServerState _currentGameState = GameServerState.Nil;

        [SerializeField] private ClientGameStateCycler _clientStateCycler = null;
        [SerializeField] private PlayerCountyAssignmentTurnCycler _countyAssignmentTurnCyclerPrefab = null;

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
            _clientStateCycler = ClientGameStateCycler.Singleton;
            SwitchToGameServerState(new WaitForPlayersServerState());
        }

        private void OnDestroy()
        {
            if (this == Singleton)
            {
                Singleton = null;
            }
        }

        [Server]
        public void SwitchToGameServerState(GameServerState gameState)
        {
            if (gameState == null)
            {
                gameState = GameServerState.Nil;
            }
            _currentGameState.OnStateDisable(this);
            _currentGameState = gameState;
            _currentGameState.OnStateEnable(this);
            _clientStateCycler.SetClientGameState(gameState.ClientStateId);
        }

        [Server]
        public PlayerCountyAssignmentTurnCycler InstantiateCountyAssignmentTurnCycler() => Instantiate(_countyAssignmentTurnCyclerPrefab.gameObject).GetComponent<PlayerCountyAssignmentTurnCycler>();
    }
}