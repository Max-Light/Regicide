using Mirror;
using System;
using System.Collections.Generic;

namespace Regicide.Game.Units
{
    public class TroopUnitRoster : NetworkBehaviour
    {
        protected List<TroopUnit> _troops = new List<TroopUnit>();

        public TroopUnit this[int index] { get => _troops[index]; }
        public IReadOnlyList<TroopUnit> Troops { get => _troops; }

        public virtual void AddTroop(TroopUnit troop) => _troops.Add(troop);
        public virtual void RemoveTroop(TroopUnit troop) => _troops.Remove(troop);
        public virtual void RemoveTroopAt(int index) => _troops.RemoveAt(index);
        public List<TroopUnit> GetTroopUnitsOfType<T>() where T : Type
        {
            List<TroopUnit> troopsOfType = new List<TroopUnit>();
            for (int troopIndex = 0; troopIndex < _troops.Count; troopIndex++)
            {
                if (_troops[troopIndex] is T)
                {
                    troopsOfType.Add(_troops[troopIndex]);
                }
            }
            return troopsOfType;
        }
    }
}