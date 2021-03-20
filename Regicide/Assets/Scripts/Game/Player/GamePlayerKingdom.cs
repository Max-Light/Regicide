
using Mirror;
using Regicide.Game.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Player
{
    public class GamePlayerKingdom : NetworkBehaviour
    {
        private class SyncEntities : SyncHashSet<uint> { }
        private SyncEntities syncOwnedEntities = new SyncEntities();

        public List<Entity> OwnedEntities { get; private set; } = new List<Entity>();

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isServer) { return; }
            syncOwnedEntities.Callback += OnOwnedEntitiesChange;
        }

        private void OnOwnedEntitiesChange(SyncEntities.Operation op, uint entityId)
        {
            switch (op) 
            {
                case SyncSet<uint>.Operation.OP_ADD:
                    if (Entity.Entities.ContainsKey(entityId))
                    {
                        OwnedEntities.Add(Entity.Entities[entityId]);
                    }
                    break;
                case SyncSet<uint>.Operation.OP_REMOVE:
                    if (Entity.Entities.ContainsKey(entityId))
                    {
                        OwnedEntities.Remove(Entity.Entities[entityId]);
                    }
                    break;
                case SyncSet<uint>.Operation.OP_CLEAR:
                    OwnedEntities.Clear();
                    break;
            }
        }

        [Server]
        public void AddEntityOwnership(Entity entity)
        {
            entity.AssignEntityOwnership(GamePlayer.Players[netId]);
            syncOwnedEntities.Add(entity.netId);
            OwnedEntities.Add(entity);
        }

        [Server]
        public void RemoveEntityOwnership(Entity entity)
        {
            entity.RemoveEntityOwnership();
            syncOwnedEntities.Remove(entity.netId);
            OwnedEntities.Remove(entity);
        }

        public List<T> GetEntitiesOfType<T>() where T : Entity
        {
            List<T> entitiesOfType = new List<T>();
            for (int entityIndex = 0; entityIndex < OwnedEntities.Count; entityIndex++)
            {
                if (OwnedEntities[entityIndex] is T entityOfType)
                {
                    entitiesOfType.Add(entityOfType);
                }
            }
            return entitiesOfType;
        }
    }
}