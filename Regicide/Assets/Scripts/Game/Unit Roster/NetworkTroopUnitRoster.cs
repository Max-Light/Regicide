using Mirror;
using System;
using System.Collections.Generic;

namespace Regicide.Game.Units
{
    public class NetworkTroopUnitRoster : TroopUnitRoster
    {
        private struct SyncTroop
        {
            public uint unitId;
            public float health;
        }

        private class SyncTroopRoster : SyncList<SyncTroop> { }
        private SyncTroopRoster _syncAvailableTroopRoster = new SyncTroopRoster();

        [Server]
        public override void AddTroop(TroopUnit troop)
        {
            base.AddTroop(troop);
            SyncTroop syncTroop = new SyncTroop
            {
                unitId = troop.UnitModel.UnitId,
                health = troop.Health
            };
            _syncAvailableTroopRoster.Add(syncTroop);
            troop.AddObserver((unit) => SynchronizeUnit((TroopUnit)unit));
        }

        [Server]
        public override void RemoveTroop(TroopUnit troop)
        {
            int index = _availableTroops.IndexOf(troop);
            if (index >= 0)
            {
                _syncAvailableTroopRoster.RemoveAt(index);
                troop.RemoveObserver((unit) => SynchronizeUnit((TroopUnit)unit));
                base.RemoveTroop(troop);
            }
        }

        [Server]
        public override void RemoveTroopAt(int index)
        {
            _syncAvailableTroopRoster.RemoveAt(index);
            TroopUnit troop = _availableTroops[index];
            troop.RemoveObserver((unit) => SynchronizeUnit((TroopUnit)unit));
            base.RemoveTroopAt(index);
        }

        private void OnTroopRosterChange(SyncTroopRoster.Operation op, int index, SyncTroop _, SyncTroop syncTroop)
        {
            switch (op)
            {
                case SyncList<SyncTroop>.Operation.OP_ADD:
                    {

                        break;
                    }
                case SyncList<SyncTroop>.Operation.OP_REMOVEAT:
                    {

                        break;
                    }
                case SyncList<SyncTroop>.Operation.OP_SET:
                    {

                        break;
                    }
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isServer) { return; }
            _syncAvailableTroopRoster.Callback += OnTroopRosterChange;
        }

        private void OnDestroy()
        {
            if (isServer) { return; }
            _syncAvailableTroopRoster.Callback -= OnTroopRosterChange;
        }

        [Server]
        private void SynchronizeUnit(TroopUnit troopUnit)
        {
            int index = _availableTroops.IndexOf(troopUnit);
            if (index > 0)
            {
                SyncTroop syncTroop = new SyncTroop
                {
                    unitId = _syncAvailableTroopRoster[index].unitId,
                    health = troopUnit.Health
                };
                _syncAvailableTroopRoster[index] = syncTroop;
            }
        }
    }
}