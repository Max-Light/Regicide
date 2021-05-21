
using Mirror;
using Regicide.Game.Units;
using System;

namespace Regicide.Game.Entities
{
    public class SpawnTroopUnitContingentCommand<T> : ICommand where T : Unit
    {
        private TroopUnitContingent _troopUnitContingent = null;
        private UnitCompanySpawnPoint _spawnPoint = null;
        private County _affiliatedCounty = null;
        private Type _unitType = null;

        public SpawnTroopUnitContingentCommand(UnitCompanySpawnPoint spawnPoint, County county)
        {
            _spawnPoint = spawnPoint;
            _troopUnitContingent = null;
            _affiliatedCounty = county;
            _unitType = typeof(T);
            
        }

        [Server]
        public void Execute()
        {
            _troopUnitContingent.transform.position = _spawnPoint.transform.position;
            NetworkServer.Spawn(_troopUnitContingent.gameObject, _affiliatedCounty.netIdentity.connectionToClient);
            _troopUnitContingent.SetAffiliatedCounty(_affiliatedCounty);
            _troopUnitContingent.SetSingularUnitType<T>();

            for (int troopIndex = 0; troopIndex < 100; troopIndex++)
            {
                TroopUnit troopUnit = Activator.CreateInstance(_unitType) as TroopUnit;
                _troopUnitContingent.TroopRoster.AddTroop(troopUnit);
            }
        }
    }
}