using Regicide.Game.Units;
using System.Collections.Generic;

namespace Regicide.Game.BattleSimulation
{
    public class TroopBattleLine
    {
        private List<TroopUnit> _troopBattleLine = null;
        private TroopBattleLineFormation _battleLineFormation = null;
        private TroopUnitRoster _troopUnitRoster = null;
        private BattleCollider _battleCollider = null;

        public TroopUnit[] BattleLineTroops { get => _troopBattleLine.ToArray(); }
        public TroopUnit this[int index] { get => _troopBattleLine[index]; }
        public BattleCollider BattleCollider { get => _battleCollider; }

        public TroopBattleLine(TroopBattleLineFormation battleLineFormation, TroopUnitRoster troopUnitRoster, BattleCollider thisBattleCollider, int battleLineCapacity)
        {
            _troopBattleLine = new List<TroopUnit>(battleLineCapacity);
            _battleLineFormation = battleLineFormation;
            _troopUnitRoster = troopUnitRoster;
            _battleCollider = thisBattleCollider;
            for (int troopIndex = 0; troopIndex < battleLineCapacity; troopIndex++)
            {
                AddTroopAt(troopIndex, troopIndex);
            }
        }

        public TroopBattleLine(TroopBattleLineFormation battleLineFormation, TroopUnitRoster troopUnitRoster, BattleCollider thisBattleCollider, List<TroopUnit> troops)
        {
            _troopBattleLine = troops;
            _battleLineFormation = battleLineFormation;
            _troopUnitRoster = troopUnitRoster;
            _battleCollider = thisBattleCollider;
        }

        private void OnTroopUnitChange(TroopUnit troopUnit)
        {
            if (troopUnit.Health == 0)
            {
                for (int troopIndex = 0; troopIndex < _troopBattleLine.Count; troopIndex++)
                {
                    if (troopUnit == _troopBattleLine[troopIndex])
                    {
                        KillTroopAt(troopIndex);
                        break;
                    }
                }
            }
        }

        private void AddTroopAt(int index, int startingSearchIndex = 0)
        {
            IReadOnlyList<TroopUnit> troopUnits = _troopUnitRoster.Troops;

            if (_battleLineFormation.IsAllTroopsActive)
            {
                if (index < _troopBattleLine.Count)
                {
                    _troopBattleLine.RemoveAt(index);
                }
            }
            else
            {
                for (int troopIndex = startingSearchIndex; troopIndex < troopUnits.Count; troopIndex++)
                {
                    if (!_battleLineFormation.IsTroopUnitActive(troopUnits[troopIndex]))
                    {
                        _battleLineFormation.SetTroopUnitAsActive(troopUnits[troopIndex], () => OnTroopUnitChange(troopUnits[troopIndex]));
                        if (index < _troopBattleLine.Count)
                        {
                            _troopBattleLine[index] = troopUnits[troopIndex];
                        }
                        else if (_troopBattleLine.Count < _troopBattleLine.Capacity)
                        {
                            _troopBattleLine.Add(troopUnits[troopIndex]);
                        }
                        break;
                    }
                }
            }
        }

        private void KillTroopAt(int index)
        {
            TroopUnit troopUnit = _troopBattleLine[index];
            _battleLineFormation.RemoveActiveTroopUnit(troopUnit);
            _troopUnitRoster.RemoveTroop(troopUnit);
            AddTroopAt(index);
        }
    }
}