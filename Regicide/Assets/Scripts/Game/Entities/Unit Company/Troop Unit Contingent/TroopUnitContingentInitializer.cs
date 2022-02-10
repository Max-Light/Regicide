
using Mirror;
using Regicide.Game.Units;
using UnityEngine;

namespace Regicide.Game.Entity
{
    public class TroopUnitContingentInitializer : NetworkBehaviour
    {
        [SerializeField] private TroopUnitContingent _troopUnitContingent = null;
        [SerializeField] private int _troopAmount = 100;

        private void SpawnTroops()
        {
            if (_troopUnitContingent.UnitModel == null)
            {
                _troopUnitContingent.SetSingularUnitType(MilitiaFootmanUnit.TroopModel);
            }
            new SpawnTroopUnitRosterCommand(_troopUnitContingent.TroopRoster, _troopUnitContingent.UnitModel.UnitId, _troopAmount).Execute();
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