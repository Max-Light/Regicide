
using Mirror;
using Regicide.Game.Entities;
using Regicide.Game.GameStates;
using Regicide.Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Regicide.Game.PlayerTurnSystem
{
    public class PlayerCountyAssignmentTurnCycler : NetworkBehaviour 
    {
        [SyncVar(hook = nameof(OnPlayerTurnChange))]
        private int playerTurn = -1;
        private int playerTurnIndex = 0;
        private List<GamePlayer> players = new List<GamePlayer>();
        private HashSet<County> availableCounties = new HashSet<County>();

        public PlayerCountyAssignmentTurnCycler Singleton { get; private set; } = null;

        private void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Debug.LogWarning("Multiple county assignment turn cyclers detected! Destroying superfluous cyclers...");
                Destroy(this);
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            InitializeTurnCycle();
        }

        public void InitializeTurnCycle()
        {
            players = GamePlayer.Players.Values.ToList();
            playerTurn = (int)players[playerTurnIndex].netId;
            CheckToStartGamePlay();
        }

        private void OnDestroy()
        {
            if (this == Singleton)
            {
                Singleton = null;
            }
        }

        private void OnPlayerTurnChange(int _, int playerId)
        {
            if (GamePlayer.Players.TryGetValue((uint)playerId, out GamePlayer player))
            {
                callback?.Invoke(player);
            }
        }

        [Client]
        public void ExecuteTurn(GamePlayer player, County county)
        {
            CmdExecuteTurn(player.netId, county.netId);
        }

        [Command]
        private void CmdExecuteTurn(uint playerId, uint countyId)
        {
            if (GamePlayer.Players.TryGetValue(playerId, out GamePlayer player) && County.Counties.TryGetValue(countyId, out County county))
            {
                if (player == CurrentPlayerTurn && !county.HasOwner())
                {
                    AssignCountyToPlayer(county);
                    ChangeTurn();
                    CheckToStartGamePlay();
                }
            }
        }

        [Server]
        private void AssignCountyToPlayer(County county)
        {
            county.AssignEntityOwnership(CurrentPlayerTurn);
        }

        [Server]
        private void ChangeTurn()
        {
            playerTurnIndex++;
            if (playerTurnIndex >= players.Count)
            {
                playerTurnIndex = 0;
            }
            playerTurn = (int)players[playerTurnIndex].netId;
        }

        [Server]
        public void SetPlayerTurn(int playerTurnIndex)
        {
            if (playerTurnIndex >= players.Count)
            {
                Debug.LogError("Index is out of bounds");
                return;
            }
            playerTurn = (int)players[playerTurnIndex].netId;
        }

        [Server]
        private void CheckToStartGamePlay()
        {
            if (AllCountiesAssigned())
            {
                ServerGameStateCycler.Singleton.SwitchToGameState(new PlayState());
            }
        }

        [Server]
        private bool AllCountiesAssigned()
        {
            foreach (County county in County.Counties.Values)
            {
                if (!county.HasOwner())
                {
                    return false;
                }
            }
            return true;
        }

        public GamePlayer CurrentPlayerTurn 
        { 
            get 
            {
                if (playerTurn >= 0 && GamePlayer.Players.TryGetValue((uint)playerTurn, out GamePlayer player))
                {
                    return player;
                }
                else
                {
                    return null;
                }
            }
        }

        private Action<GamePlayer> callback = null;

        public void AddObserver(Action<GamePlayer> observer)
        {
            callback += observer;
        }

        public void RemoveObserver(Action<GamePlayer> observer)
        {
            callback -= observer;
        }
    }
}