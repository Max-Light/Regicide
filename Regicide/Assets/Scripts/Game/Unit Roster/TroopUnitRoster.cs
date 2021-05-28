using Mirror;
using System;
using System.Collections.Generic;

namespace Regicide.Game.Units
{
    public class TroopUnitRoster : NetworkBehaviour
    {
        protected List<TroopUnit> _availableTroops = new List<TroopUnit>();

        public TroopUnit this[int index] { get => _availableTroops[index]; }
        public IReadOnlyList<TroopUnit> Troops { get => _availableTroops; }

        public virtual void AddTroop(TroopUnit troopUnit)
        {
            _availableTroops.Add(troopUnit);
            troopUnit.AddObserver((unit) => OnTroopChange((TroopUnit)unit));
        }
            
        public virtual void RemoveTroop(TroopUnit troopUnit)
        {
            if (_availableTroops.Remove(troopUnit)) 
            {
                troopUnit.RemoveObserver((unit) => OnTroopChange((TroopUnit)unit));
            }
        }

        public virtual void RemoveTroopAt(int index) 
        {
            TroopUnit troopUnit = _availableTroops[index];
            _availableTroops.RemoveAt(index);
            troopUnit.RemoveObserver((unit) => OnTroopChange((TroopUnit)unit));
        }

        public bool IsTroopAvailable(TroopUnit troopUnit) => _availableTroops.Contains(troopUnit);

        public List<TroopUnit> GetTroopUnitsOfType<T>() where T : TroopUnit
        {
            List<TroopUnit> troopsOfType = new List<TroopUnit>();
            for (int troopIndex = 0; troopIndex < _availableTroops.Count; troopIndex++)
            {
                if (_availableTroops[troopIndex] is T)
                {
                    troopsOfType.Add(_availableTroops[troopIndex]);
                }
            }
            return troopsOfType;
        }

        protected void OnTroopChange(TroopUnit troopUnit)
        {
            if (!troopUnit.IsActive)
            {
                RemoveTroop(troopUnit);
            }
        }
    }
}