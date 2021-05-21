
using Regicide.Game.Units;
using System;
using UnityEngine;

namespace Regicide.Game.Entities
{
    [RequireComponent(typeof(TroopUnitRoster))]
    public class TroopUnitContingent : UnitCompany, ISingleUnitCompanyType, ICountyUnitCompany, ITroopRosterUnitCompany
    {
        private Type _unit = null;
        private County _county = null;

        public Type UnitType => _unit;
        public County County => _county;
        public TroopUnitRoster TroopRoster { get; private set; }

        public void SetAffiliatedCounty(County county)
        {
            _county = county;
        }

        public void SetSingularUnitType<T>() where T : Unit
        {
            _unit = typeof(T);
        }

        private void Awake()
        {
            TroopRoster = GetComponent<TroopUnitRoster>();
        }
    }
}