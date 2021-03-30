
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
        private int _playerTurn = -1;
        private int _playerTurnIndex = 0;
        private List<GamePlayer> _players = new List<GamePlayer>();
        private HashSet<County> _availableCounties = new HashSet<County>();

        public static PlayerCountyAssignmentTurnCycler Singleton { get; private set; } = null;

        public GamePlayer CurrentPlayerTurn
        {
            get
            {
                if (_playerTurn >= 0 && GamePlayer.Players.TryGetValue((uint)_playerTurn, out GamePlayer player))
                {
                    return player;
                }
                else
                {
                    return null;
                }
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            InitializeTurnCycle();
        }

        public void InitializeTurnCycle()
        {
            _players = GamePlayer.Players.Values.ToList();
            _playerTurn = (int)_players[_playerTurnIndex].netId;
        }

        [Client]
        public void ExecuteTurn(GamePlayer player, County county)
        {
            CmdExecuteTurn(player.netId, county.netId);
        }

        [Server]
        public void SetPlayerTurn(int playerTurnIndex)
        {
            if (playerTurnIndex >= _players.Count)
            {
                Debug.LogError("Index is out of bounds");
                return;
            }
            _playerTurn = (int)_players[playerTurnIndex].netId;
        }

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

        private void Start()
        {
            if (isServer)
            {
                CheckToStartGamePlay();
            }
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
                
            }
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
            ICommand entityAssignment = new AssignEntityCommand(county, CurrentPlayerTurn);
            entityAssignment.Execute();
        }

        [Server]
        private void ChangeTurn()
        {
            _playerTurnIndex++;
            if (_playerTurnIndex >= _players.Count)
            {
                _playerTurnIndex = 0;
            }
            _playerTurn = (int)_players[_playerTurnIndex].netId;
        }

        [Server]
        private void CheckToStartGamePlay()
        {
            if (AllCountiesAssigned())
            {
                ServerGameStateCycler.Singleton.SwitchToGameServerState(new PlayServerState());
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
    }
}