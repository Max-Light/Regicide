
using Mirror;
using Regicide.Game.Units;

namespace Regicide.Game.Entity
{
    public class SpawnTroopUnitContingentCommand : ICommand 
    {
        private TroopUnitContingent _troopUnitContingent = null;
        private UnitCompanySpawnPoint _spawnPoint = null;
        private County _affiliatedCounty = null;
        private Unit.Model _unitModel = null;

        public SpawnTroopUnitContingentCommand(UnitCompanySpawnPoint spawnPoint, County county, Unit.Model unitModel)
        {
            _spawnPoint = spawnPoint;
            _troopUnitContingent = null;
            _affiliatedCounty = county;
            _unitModel = unitModel;
        }

        [Server]
        public void Execute()
        {
            _troopUnitContingent.transform.position = _spawnPoint.transform.position;
            NetworkServer.Spawn(_troopUnitContingent.gameObject, _affiliatedCounty.netIdentity.connectionToClient);
            _troopUnitContingent.SetAffiliatedCounty(_affiliatedCounty);
            _troopUnitContingent.SetSingularUnitType(_unitModel);
        }
    }
}