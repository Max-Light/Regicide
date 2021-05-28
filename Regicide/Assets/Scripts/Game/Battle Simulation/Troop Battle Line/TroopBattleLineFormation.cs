
using Mirror;
using Regicide.Game.Units;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    [RequireComponent(typeof(BattleEntity))]
    public class TroopBattleLineFormation : NetworkBehaviour, IBattleCommencer
    {
        private Dictionary<BattleScenario, TroopBattleLine> _battleLines = new Dictionary<BattleScenario, TroopBattleLine>();
        private HashSet<TroopUnit> _activeTroopUnits = new HashSet<TroopUnit>();
        private TroopUnitRoster _troopUnitRoster = null;

        public virtual int BattleLineID => gameObject.GetInstanceID();
        public bool IsAllTroopsActive => _troopUnitRoster.Troops.Count == _activeTroopUnits.Count;
        public TroopUnitRoster TroopUnitRoster { get => _troopUnitRoster; }
        public IReadOnlyList<IDamager> GetDamagers(BattleScenario battleScenario) => _battleLines[battleScenario].BattleLineTroops;
        public IReadOnlyList<IDamageable> GetDamageables(BattleScenario battleScenario) => _battleLines[battleScenario].BattleLineTroops;

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
                TroopBattleLine battleLine = CreateTroopBattleLine(battleCollision);
                _battleLines.Add(battleScenario, battleLine);
            }
        }

        public void DecommenceBattle(BattleEntity thisBattleEntity, BattleEntity hitBattleEntity, BattleCollision battleCollision)
        {
            foreach (TroopUnit troopUnit in _activeTroopUnits)
            {
                _troopUnitRoster.RemoveTroop(troopUnit);
            }
        }

        public bool IsTroopUnitActive(TroopUnit troopUnit) => _activeTroopUnits.Contains(troopUnit);

        public bool SetTroopUnitAsActive(TroopUnit troopUnit)
        {
            if (!_activeTroopUnits.Contains(troopUnit))
            {
                _activeTroopUnits.Add(troopUnit);
                return true;
            }
            return false;
        }

        public void RemoveActiveTroopUnit(TroopUnit troopUnit)
        {
            _activeTroopUnits.Remove(troopUnit);
        }

        private TroopBattleLine CreateTroopBattleLine(BattleCollision battleCollision)
        {
            List<TroopBattleLine> battleLinesWithCollider = new List<TroopBattleLine>();
            List<TroopUnit> troopUnits = new List<TroopUnit>();
            foreach (KeyValuePair<BattleScenario, TroopBattleLine> troopBattleLine in _battleLines)
            {
                if (troopBattleLine.Value.IsBattlingInCollider(battleCollision.ThisBattleCollider))
                {
                    battleLinesWithCollider.Add(troopBattleLine.Value);
                    troopUnits.AddRange(troopBattleLine.Value.BattleLineTroops);
                }
            }
            int troopUnitBattleLineLength = battleCollision.ThisBattleCollider.UnitLength;
            if (battleLinesWithCollider.Count == 0)
            {
                return new TroopBattleLine(this, battleCollision.ThisBattleCollider, troopUnitBattleLineLength);
            }
            float partitionedTroopAmount = Mathf.Clamp(troopUnitBattleLineLength / (battleLinesWithCollider.Count + 1), 1, troopUnitBattleLineLength);
            float amountOfTroopsUsed = 0;
            int troopStartIndex = 0;
            for (int battleLineIndex = 0; battleLineIndex < battleLinesWithCollider.Count && amountOfTroopsUsed < troopUnitBattleLineLength; battleLineIndex++)
            {
                amountOfTroopsUsed += partitionedTroopAmount;
                List<TroopUnit> newTroopBattleLine = troopUnits.GetRange(troopStartIndex, (int)(amountOfTroopsUsed - troopStartIndex));
                battleLinesWithCollider[battleLineIndex].SetBattleLine(newTroopBattleLine);
                troopStartIndex += (int)partitionedTroopAmount;
            }
            return new TroopBattleLine(this, battleCollision.ThisBattleCollider, troopUnits.GetRange(troopStartIndex, troopUnitBattleLineLength - troopStartIndex));
        }

        private void Awake()
        {
            _troopUnitRoster = GetComponent<TroopUnitRoster>();
        }
    }
}