using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public abstract class BattleBehaviour : NetworkBehaviour
    {
        protected static Dictionary<int, BattleBehaviour> _battleDamageableEntities = new Dictionary<int, BattleBehaviour>();

        public int BattleId { get => (int)netId; }

        public virtual Func<T> GetBattleResult<T>(int battleId) { return null; }

        public override void OnStartClient()
        {
            if (!_battleDamageableEntities.ContainsKey(BattleId))
            {
                _battleDamageableEntities.Add(BattleId, this);
            }
            else
            {
                Debug.LogError("Battle Behaviour already exists on " + gameObject.name);
            }
        }

        private void OnDestroy()
        {
            if (_battleDamageableEntities.TryGetValue(BattleId, out BattleBehaviour battleBehaviour) && battleBehaviour == this)
            {
                _battleDamageableEntities.Remove(BattleId);
            }
        }
    }
}