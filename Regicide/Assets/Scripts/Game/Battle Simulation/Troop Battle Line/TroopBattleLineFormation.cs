
using Mirror;
using Regicide.Game.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    [RequireComponent(typeof(BattleEntity))]
    public class TroopBattleLineFormation : NetworkBehaviour, IBattleCommencer
    {
        private Dictionary<BattleScenario, TroopBattleLine> _battleLines = new Dictionary<BattleScenario, TroopBattleLine>();
        private Dictionary<TroopUnit, Action> _activeTroopUnits = new Dictionary<TroopUnit, Action>();
        private TroopUnitRoster _troopUnitRoster = null;

        public virtual int BattleID => gameObject.GetInstanceID();
        public bool IsAllTroopsActive => _troopUnitRoster.Troops.Count == _activeTroopUnits.Count;

        public IDamager[] GetDamagers(BattleScenario battleScenario)
        {
            return _battleLines[battleScenario].BattleLineTroops;
        }

        public IDamageable[] GetDamageables(BattleScenario battleScenario)
        {
            return _battleLines[battleScenario].BattleLineTroops;
        }

        public void CommenceBattle(BattleEntity thisBattleEntity, BattleEntity hitBattleEntity, BattleCollision battleCollision)
        {
            if (_troopUnitRoster == null)
            {
                Debug.LogError("Entity troop battle line formation does not have a referenced troop roster.");
                return;
            }

            BattleSimulation.CreateBattle(this, hitBattleEntity.BattleCommencer);
            if (BattleSimulation.TryGetBattleScenario(this, hitBattleEntity.BattleCommencer, out BattleScenario battleScenario))
            {
                TroopBattleLine battleLine = GetPartitionedBattleLine(battleCollision);
                _battleLines.Add(battleScenario, battleLine);
            }
        }

        public void DecommenceBattle(BattleEntity thisBattleEntity, BattleEntity hitBattleEntity, BattleCollision battleCollision)
        {
            foreach (TroopUnit troopUnit in _activeTroopUnits.Keys)
            {
                _troopUnitRoster.RemoveTroop(troopUnit);
            }
        }

        public bool IsTroopUnitActive(TroopUnit troopUnit) => _activeTroopUnits.ContainsKey(troopUnit);

        public bool SetTroopUnitAsActive(TroopUnit troopUnit, Action action)
        {
            if (!_activeTroopUnits.ContainsKey(troopUnit))
            {
                _activeTroopUnits.Add(troopUnit, action);
                troopUnit.AddObserver(action);
                return true;
            }
            return false;
        }

        public void RemoveActiveTroopUnit(TroopUnit troopUnit)
        {
            _activeTroopUnits.Remove(troopUnit);
        }

        private TroopBattleLine GetPartitionedBattleLine(BattleCollision battleCollision)
        {
            List<BattleScenario> battlesWithCollider = new List<BattleScenario>();
            List<TroopUnit> troopUnits = new List<TroopUnit>();
            foreach (KeyValuePair<BattleScenario, TroopBattleLine> troopBattleLine in _battleLines)
            {
                if (troopBattleLine.Value.BattleCollider == battleCollision.ThisBattleCollider)
                {
                    battlesWithCollider.Add(troopBattleLine.Key);
                    troopUnits.AddRange(troopBattleLine.Value.BattleLineTroops);
                }
            }

            if (battlesWithCollider.Count == 0)
            {
                return new TroopBattleLine(this, _troopUnitRoster, battleCollision.ThisBattleCollider, battleCollision.ThisBattleCollider.UnitLength);
            }
            else
            {
                float partitionedTroopAmount = troopUnits.Count / (battlesWithCollider.Count + 1);
                float amountOfTroopsUsed = 0;
                int troopIndex = 0;
                for (int battleLineIndex = 0; battleLineIndex < battlesWithCollider.Count; battleLineIndex++)
                {
                    amountOfTroopsUsed += partitionedTroopAmount;
                    List<TroopUnit> troops = troopUnits.GetRange(troopIndex, (int)(amountOfTroopsUsed - troopIndex));
                    _battleLines[battlesWithCollider[battleLineIndex]] = new TroopBattleLine(this, _troopUnitRoster, battleCollision.ThisBattleCollider, troops);
                    troopIndex += (int)amountOfTroopsUsed;
                }
                List<TroopUnit> remainingTroops = troopUnits.GetRange(troopIndex, troopUnits.Count - troopIndex);
                return new TroopBattleLine(this, _troopUnitRoster, battleCollision.ThisBattleCollider, remainingTroops);
            }
        }

        private void Awake()
        {
            _troopUnitRoster = GetComponent<TroopUnitRoster>();
        }
    }
}