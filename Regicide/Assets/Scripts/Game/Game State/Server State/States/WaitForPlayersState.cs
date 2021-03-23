
using Mirror;
using Regicide.Game.Player;
using UnityEngine;

namespace Regicide.Game.GameStates
{
    public class WaitForPlayersState : ServerGameState
    {
        private int currentPlayerCount = 0;

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
                    currentPlayerCount++;
                    CheckToStartGame();
                    break;
                case GamePlayer.Operation.REMOVE:
                    currentPlayerCount--;
                    break;
            }
        }

        private void CheckToStartGame()
        {
            if (currentPlayerCount == NetworkServer.connections.Count)
            {
                ServerGameStateCycler.Singleton.SwitchToGameState(new SetupState());
            }
        }
    }
}