using Mirror;
using Regicide.Game.Player;
using System.Collections.Generic;

namespace Regicide.Game.Entities
{
    public abstract class Entity : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnEntityOwnerChange))]
        private int playerId = -1;

        protected GamePlayer playerOwner = null;

        public static Dictionary<uint, Entity> Entities { get; private set; } = new Dictionary<uint, Entity>();

        public override void OnStartServer()
        {
            base.OnStartServer();
            Entities.Add(netId, this);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isServer) { return; }
            Entities.Add(netId, this);
        }

        protected virtual void OnDestroy()
        {
            Entities.Remove(netId);
        }

        private void OnEntityOwnerChange(int playerId, int _)
        {
            if (playerId >= 0 && GamePlayer.Players.TryGetValue((uint)playerId, out GamePlayer player))
            {
                playerOwner = player;
            }
            else
            {
                this.playerId = -1;
                playerOwner = null;
            }
        }

        [Server]
        public void AssignEntityOwnership(GamePlayerKingdom owner)
        {
            if (owner != null)
            {
                netIdentity.AssignClientAuthority(owner.netIdentity.connectionToClient);
                playerId = (int)owner.netId;
            }
        }

        [Server]
        public void RemoveEntityOwnership()
        {
            netIdentity.RemoveClientAuthority();
            playerId = -1;
        }
    }
}