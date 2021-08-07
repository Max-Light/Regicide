
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
        private IReadOnlyList<IBattleDamageable<TroopUnitDamage>> _damageableBattleLine = null;
        private IObservable _damageableBattleLineObserver = null;

        public TroopBattleLine TroopBattleLine => _troopBattleLine;

        public TroopBattleScenario(TroopContingentBattleFormation troopContingentBattleFormation, TroopBattleLine troopBattleLine)
        {
            _troopContingentBattleFormation = troopContingentBattleFormation;
            _troopBattleLine = troopBattleLine;
            _troopBattleRoster = troopContingentBattleFormation.TroopBattleRoster;
            for (int battleUnitIndex = 0; battleUnitIndex < troopBattleLine.Count; battleUnitIndex++)
            {
                troopBattleLine[battleUnitIndex].AddObserver((battleUnit) => OnTroopBattleUnitChange(battleUnit));
            }
            _troopBattleLine.AddCallback((op, index, battleUnit) => OnBattleLineChange(op, index, battleUnit));
            FillBattleLine();
        }

        public void InitializeDamageableBattleLine(IReadOnlyList<IBattleDamageable<TroopUnitDamage>> damageableBattleLineUnits, IObservable observerableObject)
        {
            _damageableBattleLine = damageableBattleLineUnits;
            _damageableBattleLineObserver = observerableObject;
        }

        public override void StartBattle()
        {
            Debug.Log("Starting Battle");
            FillBattleLine();
            if (_troopBattleLine.UnitBattleLine.Count != 0 && _damageableBattleLine.Count != 0)
            {
                StartTroopUnitDamageBattleLineUpdates(_damageableBattleLine);
                _damageableBattleLineObserver.AddObserver(() => OnDamageableBattleLineLengthChange());
                _troopBattleLine.AddObserver(() => OnDamagerBattleLineLengthChange());
            }
            Debug.Log(_troopBattleLine.UnitBattleLine.Count + " vs. " + _damageableBattleLine.Count);
        }

        public override void StopBattle()
        {
            Debug.Log("Stopping Battle");
            StopTroopUnitDamageBattleLineUpdates();
            _troopBattleLine.Clear();
            _damageableBattleLineObserver.RemoveObserver(() => OnDamageableBattleLineLengthChange());
            _troopBattleLine.RemoveObserver(() => OnDamagerBattleLineLengthChange());
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
                if (_battleUpdates.TryGetValue(_troopBattleLine[troopIndex], out TroopUnitDamageBattleUpdate battleUpdate))
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
                    _battleUpdates.Add(_troopBattleLine[troopIndex], battleUpdate);
                    battleUpdate.StartBattleUpdate(_troopBattleLine[troopIndex]);
                }
            }
        }

        private void StopTroopUnitDamageBattleLineUpdates()
        {
            for (int troopIndex = 0; troopIndex < _troopBattleLine.Count; troopIndex++)
            {
                if (_battleUpdates.TryGetValue(_troopBattleLine[troopIndex], out TroopUnitDamageBattleUpdate battleUpdate))
                {
                    battleUpdate.StopBattleUpdate();
                    _battleUpdates.Remove(_troopBattleLine[troopIndex]);
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

        private void OnBattleLineChange(BattleLine<TroopBattleUnit>.Operation op, int _, TroopBattleUnit battleUnit)
        {
            switch (op) 
            {
                case BattleLine<TroopBattleUnit>.Operation.OP_ADD:
                case BattleLine<TroopBattleUnit>.Operation.OP_INSERT:
                    {
                        battleUnit.AddObserver((battleUnit) => OnTroopBattleUnitChange(battleUnit));
                        break;
                    }
                case BattleLine<TroopBattleUnit>.Operation.OP_REMOVEAT:
                    {
                        battleUnit.RemoveObserver((battleUnit) => OnTroopBattleUnitChange(battleUnit));
                        if (_battleUpdates.TryGetValue(battleUnit, out TroopUnitDamageBattleUpdate battleUpdate))
                        {
                            battleUpdate.SetDamageables();
                            _battleUpdates.Remove(battleUnit);
                        }
                        break;
                    }
            }
        }

        private void OnDamagerBattleLineLengthChange()
        {
            StartTroopUnitDamageBattleLineUpdates(_damageableBattleLine);
            Debug.Log(_troopBattleLine.UnitBattleLine.Count + " vs. " + _damageableBattleLine.Count);
        }

        private void OnDamageableBattleLineLengthChange()
        {
            StartTroopUnitDamageBattleLineUpdates(_damageableBattleLine);
        }
    }
}