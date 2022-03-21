
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Player
{
    public class GamePlayer : NetworkBehaviour
    {
        [SerializeField] private Canvas _playerCanvas = null;

        private static GamePlayer _localGamePlayer = null;

        private PlayerCameraController _playerCameraController = null;

        public string PlayerName { get; private set; } = "Player";
        public ulong SteamID { get; private set; } = 0;

        public static GamePlayer LocalGamePlayer { get => _localGamePlayer; }
        public static Dictionary<uint, GamePlayer> Players { get; private set; } = new Dictionary<uint, GamePlayer>();

        public PlayerCameraController PlayerCameraController { get => _playerCameraController; }
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

        protected void CreatePlayerCamera()
        {
            _playerCameraController = new PlayerCameraController();
            _playerCameraController.Enable();
        }

        public override void OnStartLocalPlayer()
        {
            _localGamePlayer = this;
            _playerCanvas.gameObject.SetActive(true);
            CreatePlayerCamera();
            for (int cameraIndex = 0; cameraIndex < Camera.allCamerasCount; cameraIndex++)
            {
                Camera.allCameras[cameraIndex].enabled = false;
            }
        }

        public override void OnStartClient()
        {
            Players.Add(netIdentity.netId, this);
            callback?.Invoke(Operation.ADD, this);
        }

        private void Awake()
        {
            PlayerKingdom = GetComponent<GamePlayerKingdom>();
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