using Regicide.Game.Units;
using System;

namespace Regicide.Game.Entity
{
    public interface ISingleUnitCompanyType
    {
        Unit.Model UnitModel { get; }
        void SetSingularUnitType(Unit.Model unitModel);
    }
}