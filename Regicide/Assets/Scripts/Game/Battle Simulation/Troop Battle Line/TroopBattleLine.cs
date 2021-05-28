using Regicide.Game.Units;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class TroopBattleLine
    {
        private List<TroopUnit> _troopBattleLine = null;
        private int _battleLineCapacity = 0;
        private TroopBattleLineFormation _battleLineFormation = null;
        private BattleCollider _battleCollider = null;

        public List<TroopUnit> BattleLineTroops { get => _troopBattleLine; }

        public TroopBattleLine(TroopBattleLineFormation battleLineFormation, BattleCollider thisBattleCollider, int battleLineCapacity)
        {
            _troopBattleLine = new List<TroopUnit>(battleLineCapacity);
            _battleLineCapacity = battleLineCapacity;
            _battleLineFormation = battleLineFormation;
            _battleCollider = thisBattleCollider;
            FillBattleLine();
        }

        public TroopBattleLine(TroopBattleLineFormation battleLineFormation, BattleCollider thisBattleCollider, List<TroopUnit> troops)
        {
            _troopBattleLine = troops;
            _battleLineCapacity = troops.Count;
            _battleLineFormation = battleLineFormation;
            _battleCollider = thisBattleCollider;
            FillBattleLine();
        }

        public void SetBattleLine(List<TroopUnit> troopUnits)
        {
            _troopBattleLine = troopUnits;
            _battleLineCapacity = troopUnits.Count;
        }

        public bool IsBattlingInCollider(BattleCollider collider)
        {
            return _battleCollider == collider;
        }

        private void FillBattleLine()
        {
            IReadOnlyList<TroopUnit> troopUnits = _battleLineFormation.TroopUnitRoster.Troops;
            for (int troopIndex = 0; troopIndex < troopUnits.Count && _troopBattleLine.Count < _battleLineCapacity; troopIndex++)
            {
                if (!_battleLineFormation.IsTroopUnitActive(troopUnits[troopIndex]))
                {
                    _troopBattleLine.Add(troopUnits[troopIndex]);
                    _battleLineFormation.SetTroopUnitAsActive(troopUnits[troopIndex]);
                    troopUnits[troopIndex].AddObserver((unit) => OnTroopUnitChange((TroopUnit)unit));
                }
            }
        }

        private void ReplaceUnitAt(int index)
        {
            _battleLineFormation.RemoveActiveTroopUnit(_troopBattleLine[index]);
            _troopBattleLine[index].RemoveObserver((unit) => OnTroopUnitChange((TroopUnit)unit));
            TroopUnit troopUnit = GetAvailavleTroopUnit();
            if (troopUnit != null)
            {
                _battleLineFormation.SetTroopUnitAsActive(troopUnit);
                troopUnit.AddObserver((unit) => OnTroopUnitChange((TroopUnit)unit));
                _troopBattleLine[index] = troopUnit;
            }
            else
            {
                _troopBattleLine.RemoveAt(index);
            }
        }

        private TroopUnit GetAvailavleTroopUnit()
        {
            IReadOnlyList<TroopUnit> troopUnits = _battleLineFormation.TroopUnitRoster.Troops;
            if (_battleLineFormation.IsAllTroopsActive)
            {
                return null;
            }
            for (int troopUnitIndex = 0; troopUnitIndex < troopUnits.Count; troopUnitIndex++)
            {
                if (!_battleLineFormation.IsTroopUnitActive(troopUnits[troopUnitIndex]))
                {
                    return troopUnits[troopUnitIndex];
                }
            }
            return null;
        }

        private void OnTroopUnitChange(TroopUnit troopUnit)
        {
            if (!troopUnit.IsActive)
            {
                int indexOfTroopUnit = _troopBattleLine.IndexOf(troopUnit);
                if (indexOfTroopUnit >= 0)
                {
                    ReplaceUnitAt(indexOfTroopUnit);
                }
            }
        }
    }
}