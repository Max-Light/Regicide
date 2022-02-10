
using Regicide.Game.Units;
using UnityEngine;

namespace Regicide.Game.Entity
{
    [RequireComponent(typeof(TroopUnitRoster))]
    public class TroopUnitContingent : UnitCompany, ISingleUnitCompanyType, ICountyUnitCompany, ITroopRosterUnitCompany
    {
        public County County { get; private set; }
        public TroopUnitRoster TroopRoster { get; private set; }
        public Unit.Model UnitModel { get; private set; }

        public void SetAffiliatedCounty(County county)
        {
            County = county;
        }

        public void SetSingularUnitType(Unit.Model unitModel)
        {
            UnitModel = unitModel;
        }

        private void Awake()
        {
            TroopRoster = GetComponent<TroopUnitRoster>();
        }
    }
}