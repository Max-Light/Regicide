using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private SyncTroopRoster _syncTroopRoster = new SyncTroopRoster();

        private Dictionary<TroopUnit, Action> _syncTroopUnitChangeActions = new Dictionary<TroopUnit, Action>();

        [Server]
        public override void AddTroop(TroopUnit troop)
        {
            base.AddTroop(troop);
            SyncTroop syncTroop = new SyncTroop
            {
                unitId = troop.Model.UnitId,
                health = troop.Health
            };
            _syncTroopRoster.Add(syncTroop);
            Action syncTroopUnitAction = () => SynchronizeUnit(troop);
            _syncTroopUnitChangeActions.Add(troop, syncTroopUnitAction);
            troop.AddObserver(syncTroopUnitAction);
        }

        [Server]
        public override void RemoveTroop(TroopUnit troop)
        {
            int index = _troops.IndexOf(troop);
            if (index >= 0)
            {
                _syncTroopRoster.RemoveAt(index);
                troop.RemoveObserver(_syncTroopUnitChangeActions[troop]);
                _syncTroopUnitChangeActions.Remove(troop);
                base.RemoveTroop(troop);
            }
        }

        [Server]
        public override void RemoveTroopAt(int index)
        {
            _syncTroopRoster.RemoveAt(index);
            TroopUnit troop = _troops[index];
            troop.RemoveObserver(_syncTroopUnitChangeActions[troop]);
            _syncTroopUnitChangeActions.Remove(troop);
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
            _syncTroopRoster.Callback += OnTroopRosterChange;
        }

        private void OnDestroy()
        {
            _syncTroopRoster.Callback -= OnTroopRosterChange;
        }

        [Server]
        private void SynchronizeUnit(TroopUnit troopUnit)
        {
            int index = _troops.IndexOf(troopUnit);
            if (index > 0)
            {
                SyncTroop syncTroop = new SyncTroop
                {
                    unitId = _syncTroopRoster[index].unitId,
                    health = troopUnit.Health
                };
                _syncTroopRoster[index] = syncTroop;
            }
        }
    }
}