
using Mirror;
using Regicide.Game.Player;
using UnityEngine;

namespace Regicide.Game.GameStates
{
    public class WaitForPlayersServerState : GameServerState
    {
        private int _currentPlayerCount = 0;

        public WaitForPlayersServerState()
        {
            ClientStateId = new WaitForPlayersClientState().GameStateId;
        }

        public override void OnStateEnable(ServerGameStateCycler cycler)
        {
            base.OnStateEnable(cycler);
            Debug.Log("Waiting for players");
            GamePlayer.AddObserver(OnGamePlayersChange);
        }

        public override void OnStateDisable(ServerGameStateCycler cycler)
        {
            base.OnStateDisable(cycler);
            GamePlayer.RemoveObserver(OnGamePlayersChange);
        }

        private void OnGamePlayersChange(GamePlayer.Operation op, GamePlayer _)
        {
            switch (op) 
            {
                case GamePlayer.Operation.ADD:
                    _currentPlayerCount++;
                    CheckToStartGame();
                    break;
                case GamePlayer.Operation.REMOVE:
                    _currentPlayerCount--;
                    break;
            }
        }

        private void CheckToStartGame()
        {
            if (_currentPlayerCount == NetworkServer.connections.Count)
            {
                ServerGameStateCycler.Singleton.SwitchToGameServerState(new SetupServerState());
            }
        }
    }
}