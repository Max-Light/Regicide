
using Regicide.Game.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class TroopBattleScenario : BattleScenario
    {
        private TroopContingentBattleSimulator _troopContingentBattleFormation = null;
        private TroopBattleLine _troopBattleLine = null;
        private TroopBattleRoster _troopBattleRoster = null;
        private Dictionary<TroopBattleUnit, TroopUnitDamageBattleUpdate> _battleUpdates = new Dictionary<TroopBattleUnit, TroopUnitDamageBattleUpdate>();
        private IBattleLineDamageable<TroopUnitDamage> _damageableBattleLine = null;

        public TroopBattleLine TroopBattleLine => _troopBattleLine;
        public IBattleLineDamageable<TroopUnitDamage> DamageableBattleLine { get => _damageableBattleLine; }

        public TroopBattleScenario(TroopContingentBattleSimulator troopContingentBattleFormation, TroopBattleLine troopBattleLine)
        {
            _troopContingentBattleFormation = troopContingentBattleFormation;
            _troopBattleLine = troopBattleLine;
            _troopBattleRoster = troopContingentBattleFormation.TroopBattleRoster;
            for (int battleUnitIndex = 0; battleUnitIndex < troopBattleLine.Count; battleUnitIndex++)
            {
                (troopBattleLine[battleUnitIndex]).AddObserver((battleUnit) => OnTroopBattleUnitChange(battleUnit));
            }
            _troopBattleLine.AddBattleUnitCallback((op, index, battleUnit) => OnBattleLineChange(op, index, battleUnit));
            _troopBattleLine.FillBattleLine(_troopBattleRoster);
        }

        public void InitializeDamageableBattleLine(IBattleLineDamageable<TroopUnitDamage> damageableBattleLine)
        {
            _damageableBattleLine = damageableBattleLine;
        }

        public override void StartBattle()
        {
            base.StartBattle();
            _troopBattleLine.FillBattleLine(_troopBattleRoster);
            Debug.Log("Starting Battle on " + _troopContingentBattleFormation.gameObject.name + ": " + _troopBattleLine.UnitBattleLine.Count + " (" + _troopBattleLine.BattleLineLength + ")" + " vs. " + _damageableBattleLine.BattleLineDamageable.Count);
            if (_troopBattleLine.UnitBattleLine.Count != 0 && _damageableBattleLine.BattleLineDamageable.Count != 0)
            {
                AssignBattleUpdates();
                _troopBattleLine.AddBattleLineLengthCallback(() => OnBattleLineDamagerCountChange());
                _damageableBattleLine.AddBattleLineLengthCallback(() => OnBattleLineDamageableCountChange());
            }
        }

        public override void StopBattle()
        {
            base.StopBattle();
            ClearBattleUpdates();
            _troopBattleLine.Clear();
            _troopBattleLine.RemoveBattleLineLengthCallback(() => OnBattleLineDamagerCountChange());
            _damageableBattleLine.RemoveBattleLineLengthCallback(() => OnBattleLineDamageableCountChange());
            Debug.Log("Stopping Battle");
        }

        public override void EndBattle()
        {
            base.EndBattle();
            _troopBattleLine.RemoveBattleUnitCallback((op, index, battleUnit) => OnBattleLineChange(op, index, battleUnit));
        }

        private void AssignBattleUpdates()
        {
            IBattleDamageable<TroopUnitDamage>[][] partitionedDamageableBattleLine = BattleLine.PartitionBattleLine(_damageableBattleLine.BattleLineDamageable, _troopBattleLine.Count);
            for (int troopIndex = 0; troopIndex < _troopBattleLine.Count; troopIndex++)
            {
                TroopBattleUnit troopBattleUnit = _troopBattleLine[troopIndex];
                if (_battleUpdates.TryGetValue(troopBattleUnit, out TroopUnitDamageBattleUpdate battleUpdate))
                {
                    battleUpdate.BattleDamageables = partitionedDamageableBattleLine[troopIndex];
                }
                else
                {
                    battleUpdate = new TroopUnitDamageBattleUpdate(_troopContingentBattleFormation, troopBattleUnit, partitionedDamageableBattleLine[troopIndex]);
                    _battleUpdates.Add(troopBattleUnit, battleUpdate);
                    battleUpdate.Start();
                }
            }
        }

        private void ClearBattleUpdates()
        {
            for (int troopIndex = 0; troopIndex < _troopBattleLine.Count; troopIndex++)
            {
                TroopBattleUnit troopBattleUnit = _troopBattleLine[troopIndex];
                if (_battleUpdates.TryGetValue(troopBattleUnit, out TroopUnitDamageBattleUpdate battleUpdate))
                {
                    battleUpdate.Stop();
                }
            }
            _battleUpdates.Clear();
        }

        private void OnTroopBattleUnitChange(TroopBattleUnit troopBattleUnit)
        {
            if (!troopBattleUnit.TroopUnit.IsAlive)
            {
                _troopContingentBattleFormation.StartCoroutine(TroopUnitChangeDelay(troopBattleUnit));
            }
        }

        private IEnumerator TroopUnitChangeDelay(TroopBattleUnit troopBattleUnit)
        {
            yield return new WaitForEndOfFrame();
            _troopBattleRoster.RemoveAsBattleActive(troopBattleUnit.TroopUnit);
            if (_troopBattleRoster.TrySelectBattleActiveTroopUnit(out TroopUnit availableTroopUnit))
            {
                troopBattleUnit.SetTroopUnit(availableTroopUnit);
                yield return new WaitForSeconds(0.75f);
                if (_battleUpdates.TryGetValue(troopBattleUnit, out TroopUnitDamageBattleUpdate battleUpdate))
                {
                    battleUpdate.Start();
                }
            }
            else
            {
                _troopBattleLine.Remove(troopBattleUnit);
                if (_battleUpdates.TryGetValue(troopBattleUnit, out TroopUnitDamageBattleUpdate battleUpdate))
                {
                    battleUpdate.Stop();
                    _battleUpdates.Remove(troopBattleUnit);
                }
            }
        }

        private void OnBattleLineChange(BattleLine<TroopBattleUnit>.CallbackOperation op, int _, TroopBattleUnit troopBattleUnit)
        {
            switch (op) 
            {
                case BattleLine<TroopBattleUnit>.CallbackOperation.OP_ADD:
                case BattleLine<TroopBattleUnit>.CallbackOperation.OP_INSERT:
                    {
                        troopBattleUnit.AddObserver((battleUnit) => OnTroopBattleUnitChange(battleUnit));
                        break;
                    }
                case BattleLine<TroopBattleUnit>.CallbackOperation.OP_REMOVEAT:
                    {
                        troopBattleUnit.RemoveObserver((battleUnit) => OnTroopBattleUnitChange(battleUnit));
                        if (_battleUpdates.TryGetValue(troopBattleUnit, out TroopUnitDamageBattleUpdate battleUpdate))
                        {
                            battleUpdate.Stop();
                            _battleUpdates.Remove(troopBattleUnit);
                        }
                        break;
                    }
            }
        }

        private void OnBattleLineDamagerCountChange()
        {
            AssignBattleUpdates();
            Debug.Log(_troopContingentBattleFormation.gameObject + ": " + _troopBattleLine.UnitBattleLine.Count + " vs. " + _damageableBattleLine.BattleLineDamageable.Count);
        }

        private void OnBattleLineDamageableCountChange()
        {
            AssignBattleUpdates();
            if (_damageableBattleLine.BattleLineDamageable.Count == 0)
            {
                Debug.Log(_troopContingentBattleFormation.gameObject + " >>> Troops Remaining: " + _troopBattleRoster.GetComponent<TroopUnitRoster>().Troops.Count);
            }
        }
    }
}