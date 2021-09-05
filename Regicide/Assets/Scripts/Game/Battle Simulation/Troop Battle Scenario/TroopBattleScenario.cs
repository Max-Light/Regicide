
using Regicide.Game.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class TroopBattleScenario : BattleScenario
    {
        private TroopContingentBattleFormation _troopContingentBattleFormation = null;
        private TroopBattleLine _troopBattleLine = null;
        private TroopBattleRoster _troopBattleRoster = null;
        private Dictionary<TroopBattleUnit, TroopUnitDamageBattleUpdate> _battleUpdates = new Dictionary<TroopBattleUnit, TroopUnitDamageBattleUpdate>();
        private IBattleLineDamageable<TroopUnitDamage> _damageableBattleLine = null;

        public TroopBattleLine TroopBattleLine => _troopBattleLine;

        public TroopBattleScenario(TroopContingentBattleFormation troopContingentBattleFormation, TroopBattleLine troopBattleLine)
        {
            _troopContingentBattleFormation = troopContingentBattleFormation;
            _troopBattleLine = troopBattleLine;
            _troopBattleRoster = troopContingentBattleFormation.TroopBattleRoster;
            for (int battleUnitIndex = 0; battleUnitIndex < troopBattleLine.Count; battleUnitIndex++)
            {
                ((TroopBattleUnit)troopBattleLine[battleUnitIndex]).AddObserver((battleUnit) => OnTroopBattleUnitChange(battleUnit));
            }
            _troopBattleLine.AddCallback((op, index, battleUnit) => OnBattleLineChange(op, index, battleUnit));
            FillBattleLine();
        }

        public void InitializeDamageableBattleLine(IBattleLineDamageable<TroopUnitDamage> damageableBattleLine)
        {
            _damageableBattleLine = damageableBattleLine;
        }

        public override void StartBattle()
        {
            Debug.Log("Starting Battle");
            FillBattleLine();
            if (_troopBattleLine.UnitBattleLine.Count != 0 && _damageableBattleLine.BattleLineDamageable.Count != 0)
            {
                StartTroopUnitDamageBattleLineUpdates(_damageableBattleLine.BattleLineDamageable);
                _troopBattleLine.AddCallback((op, index, battleUnit) => OnBattleLineDamagerCountChange(op, index, battleUnit));
                _damageableBattleLine.AddCallback((op, index, battleUnit) => OnBattleLineDamageableCountChange(op, index, battleUnit));
            }
            Debug.Log(_troopBattleLine.UnitBattleLine.Count + " vs. " + _damageableBattleLine.BattleLineDamageable.Count);
        }

        public override void StopBattle()
        {
            Debug.Log("Stopping Battle");
            StopTroopUnitDamageBattleLineUpdates();
            _troopBattleLine.Clear();
            _troopBattleLine.RemoveCallback((op, index, battleUnit) => OnBattleLineDamagerCountChange(op, index, battleUnit));
            _damageableBattleLine.RemoveCallback((op, index, battleUnit) => OnBattleLineDamageableCountChange(op, index, battleUnit));
        }

        public override void EndBattle()
        {
            StopBattle();
            _troopBattleLine.RemoveCallback((op, index, battleUnit) => OnBattleLineChange(op, index, battleUnit));
        }

        private void FillBattleLine()
        {
            for (int battleIndex = _troopBattleLine.Count; battleIndex < _troopBattleLine.BattleLineLength; battleIndex++)
            {
                if (_troopBattleRoster.TryGetAvilableTroopUnit(out TroopUnit troopUnit))
                {
                    TroopBattleUnit troopBattleUnit = new TroopBattleUnit(troopUnit);
                    _troopBattleLine.Add(troopBattleUnit);
                }
                else
                {
                    break;
                }
            }
        }

        private void StartTroopUnitDamageBattleLineUpdates(IReadOnlyList<IBattleDamageable<TroopUnitDamage>> damageables)
        {
            IBattleDamageable<TroopUnitDamage>[][] partitionedDamageableBattleLine = BattleLine<IBattleDamageable<TroopUnitDamage>>.PartitionBattleLine(damageables, _troopBattleLine.Count);
            for (int troopIndex = 0; troopIndex < _troopBattleLine.Count; troopIndex++)
            {
                TroopBattleUnit troopBattleUnit = (TroopBattleUnit)_troopBattleLine[troopIndex];
                if (_battleUpdates.TryGetValue(troopBattleUnit, out TroopUnitDamageBattleUpdate battleUpdate))
                {
                    if (damageables == null)
                    {
                        battleUpdate.StopBattleUpdate();
                    }
                    else
                    {
                        battleUpdate.SetDamageables(partitionedDamageableBattleLine[troopIndex]);
                    }
                }
                else
                {
                    battleUpdate = new TroopUnitDamageBattleUpdate(_troopContingentBattleFormation, partitionedDamageableBattleLine[troopIndex]);
                    _battleUpdates.Add(troopBattleUnit, battleUpdate);
                    battleUpdate.StartBattleUpdate(troopBattleUnit);
                }
            }
        }

        private void StopTroopUnitDamageBattleLineUpdates()
        {
            for (int troopIndex = 0; troopIndex < _troopBattleLine.Count; troopIndex++)
            {
                TroopBattleUnit troopBattleUnit = (TroopBattleUnit)_troopBattleLine[troopIndex];
                if (_battleUpdates.TryGetValue(troopBattleUnit, out TroopUnitDamageBattleUpdate battleUpdate))
                {
                    battleUpdate.StopBattleUpdate();
                    _battleUpdates.Remove(troopBattleUnit);
                }
            }
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
            if (_troopBattleRoster.TryGetAvilableTroopUnit(out TroopUnit availableTroopUnit))
            {
                _troopBattleRoster.AddAsBattleActive(availableTroopUnit);
                troopBattleUnit.SetTroopUnit(availableTroopUnit);
                yield return new WaitForSeconds(0.75f);
                if (_battleUpdates.TryGetValue(troopBattleUnit, out TroopUnitDamageBattleUpdate battleUpdate))
                {
                    battleUpdate.StartBattleUpdate(troopBattleUnit);
                }
            }
            else
            {
                _troopBattleLine.Remove(troopBattleUnit);
                if (_battleUpdates.TryGetValue(troopBattleUnit, out TroopUnitDamageBattleUpdate battleUpdate))
                {
                    battleUpdate.StopBattleUpdate();
                }
            }
        }

        private void OnBattleLineChange(BattleLineOperation op, int _, IBattleUnit battleUnit)
        {
            TroopBattleUnit troopBattleUnit = (TroopBattleUnit)battleUnit;
            switch (op) 
            {
                case BattleLineOperation.OP_ADD:
                case BattleLineOperation.OP_INSERT:
                    {
                        troopBattleUnit.AddObserver((battleUnit) => OnTroopBattleUnitChange(battleUnit));
                        break;
                    }
                case BattleLineOperation.OP_REMOVEAT:
                    {
                        troopBattleUnit.RemoveObserver((battleUnit) => OnTroopBattleUnitChange(battleUnit));
                        if (_battleUpdates.TryGetValue(troopBattleUnit, out TroopUnitDamageBattleUpdate battleUpdate))
                        {
                            battleUpdate.SetDamageables();
                            _battleUpdates.Remove(troopBattleUnit);
                        }
                        break;
                    }
            }
        }

        private void OnBattleLineDamagerCountChange(BattleLineOperation op, int _, IBattleUnit battleUnit)
        {
            StartTroopUnitDamageBattleLineUpdates(_damageableBattleLine.BattleLineDamageable);
            Debug.Log(_troopBattleLine.UnitBattleLine.Count + " vs. " + _damageableBattleLine.BattleLineDamageable.Count);
        }

        private void OnBattleLineDamageableCountChange(BattleLineOperation op, int _, IBattleUnit battleUnit)
        {
            StartTroopUnitDamageBattleLineUpdates(_damageableBattleLine.BattleLineDamageable);
        }
    }
}