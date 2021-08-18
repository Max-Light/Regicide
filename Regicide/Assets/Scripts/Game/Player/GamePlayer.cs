
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Player
{
    public class GamePlayer : NetworkBehaviour
    {
        private static GamePlayer _localGamePlayer = null;
        [SerializeField] private Camera _playerCamera = null;
        [SerializeField] private Canvas _playerCanvas = null;

        public string PlayerName { get; private set; } = "Player";
        public ulong SteamID { get; private set; } = 0;

        public static GamePlayer LocalGamePlayer { get => _localGamePlayer; }
        public static Dictionary<uint, GamePlayer> Players { get; private set; } = new Dictionary<uint, GamePlayer>();
        public GamePlayerKingdom PlayerKingdom { get; private set; } = null;

        public enum Operation { ADD, REMOVE }
        private static Action<Operation, GamePlayer> callback = null;

        public static void AddObserver(Action<Operation, GamePlayer> observer)
        {
            callback += observer;
        }

        public static void RemoveObserver(Action<Operation, GamePlayer> observer)
        {
            callback -= observer;
        }

        public override void OnStartLocalPlayer()
        {
            _localGamePlayer = this;
            _playerCamera.gameObject.SetActive(true);
            _playerCanvas.gameObject.SetActive(true);
            for (int cameraIndex = 0; cameraIndex < Camera.allCamerasCount; cameraIndex++)
            {
                Camera.allCameras[cameraIndex].enabled = false;
            }
            _playerCamera.enabled = true;
        }

        public override void OnStartClient()
        {
            Players.Add(netIdentity.netId, this);
            callback?.Invoke(Operation.ADD, this);
        }

        private void Awake()
        {
            PlayerKingdom = GetComponent<GamePlayerKingdom>();
            _playerCamera.gameObject.SetActive(false);
            _playerCanvas.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_localGamePlayer == this)
            {
                _localGamePlayer = null;
                Players.Remove(netIdentity.netId);
                callback?.Invoke(Operation.REMOVE, this);
            }
        }
    }
}