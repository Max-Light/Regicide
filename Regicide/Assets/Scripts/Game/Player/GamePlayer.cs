
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Player
{
    public class GamePlayer : NetworkBehaviour
    {
        public string PlayerName { get; private set; } = "Player";
        public ulong SteamID { get; private set; } = 0;

        public static GamePlayer LocalPlayer { get; private set; } = null;
        public static Dictionary<uint, GamePlayer> Players { get; private set; } = new Dictionary<uint, GamePlayer>();

        [SerializeField] private Camera playerCamera = null;
        [SerializeField] private Canvas playerCanvas = null;

        public override void OnStartServer()
        {
            base.OnStartServer();
            Players.Add(netIdentity.netId, this);
        }

        public override void OnStartAuthority()
        {
            base.OnStopAuthority();
            LocalPlayer = this;
            playerCamera.gameObject.SetActive(true);
            playerCanvas.gameObject.SetActive(true);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isServer)
            {
                return;
            }
            Players.Add(netIdentity.netId, this);
        }

        private void OnDestroy()
        {
            if (LocalPlayer == this)
            {
                LocalPlayer = null;
                Players.Remove(netIdentity.netId);
            }
        }
    }
}