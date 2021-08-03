using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public abstract class BattleObject : NetworkBehaviour
    {
        protected static Dictionary<int, BattleObject> _battleDamageableEntities = new Dictionary<int, BattleObject>();

        public int BattleId { get => (int)netId; }

        public virtual bool TryGetBattleLineResult<T>(int battleId, out Func<IReadOnlyList<T>> battleLineResult) { battleLineResult = null; return false; } 
        public virtual bool TryGetBattleUnitResult<T>(int battleId, out Func<T> battleUnitResult) { battleUnitResult = null; return false; }
        public virtual bool TryGetBattleLineObserver(int battleId, out IObservable observerableObject) { observerableObject = null; return false; }

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
            if (_battleDamageableEntities.TryGetValue(BattleId, out BattleObject battleBehaviour) && battleBehaviour == this)
            {
                _battleDamageableEntities.Remove(BattleId);
            }
        }
    }
}