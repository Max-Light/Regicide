
using Regicide.Game.Units;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class TroopBattleRoster : MonoBehaviour
    {
        [SerializeField] private TroopUnitRoster _troopUnitRoster = null;
        private HashSet<TroopUnit> _activeTroopBattleUnits = new HashSet<TroopUnit>();

        public bool AllTroopsBattleActive => _troopUnitRoster.Troops.Count == _activeTroopBattleUnits.Count;
        public void AddAsBattleActive(TroopUnit troopUnit) => _activeTroopBattleUnits.Add(troopUnit);
        public bool RemoveAsBattleActive(TroopUnit troopUnit) => _activeTroopBattleUnits.Remove(troopUnit);
        public bool ContainsBattleActiveTroopUnit(TroopUnit troopUnit) => _activeTroopBattleUnits.Contains(troopUnit);

        public bool TrySelectBattleActiveTroopUnit(out TroopUnit troopUnit)
        {
            if (!AllTroopsBattleActive)
            {
                IReadOnlyList<TroopUnit> troopRoster = _troopUnitRoster.Troops;
                for (int troopIndex = 0; troopIndex < troopRoster.Count; troopIndex++)
                {
                    if (!_activeTroopBattleUnits.Contains(troopRoster[troopIndex]))
                    {
                        _activeTroopBattleUnits.Add(troopRoster[troopIndex]);
                        troopUnit = troopRoster[troopIndex];
                        return true;
                    }
                }
            }
            troopUnit = null;
            return false; 
        }

        private void OnValidate()
        {
            _troopUnitRoster = GetComponent<TroopUnitRoster>();
        }
    }
}