
using Mirror;
using Regicide.Game.Units;
using UnityEngine;

namespace Regicide.Game.Entities
{
    public class TroopUnitContingentInitializer : NetworkBehaviour
    {
        [SerializeField] private TroopUnitContingent _troopUnitContingent = null;

        private void SpawnTroops()
        {
            if (_troopUnitContingent.UnitModel == null)
            {
                _troopUnitContingent.SetSingularUnitType(MilitiaFootmanUnit.TroopModel);
            }
            new SpawnTroopUnitRosterCommand(_troopUnitContingent.TroopRoster, _troopUnitContingent.UnitModel.UnitId, 100).Execute();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SpawnTroops();
            if (isClient) { return; }
            Destroy(this);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Destroy(this);
        }

        private void OnValidate()
        {
            _troopUnitContingent = GetComponent<TroopUnitContingent>();
        }
    }
}