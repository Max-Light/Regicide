using Mirror;
using System.Collections.Generic;

namespace Regicide.Game.Units
{
    public class TroopUnitRoster : NetworkBehaviour
    {
        protected List<TroopUnit> _troops = new List<TroopUnit>();

        public IReadOnlyCollection<TroopUnit> Troops { get => _troops; }

        public virtual void AddTroop(TroopUnit troop) => _troops.Add(troop);
        public virtual void RemoveTroop(TroopUnit troop) => _troops.Remove(troop);
        public virtual void RemoveTroopAt(int index) => _troops.RemoveAt(index);
    }
}