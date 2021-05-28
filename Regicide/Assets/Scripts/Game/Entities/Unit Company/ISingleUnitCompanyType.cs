using Regicide.Game.Units;
using System;

namespace Regicide.Game.Entities
{
    public interface ISingleUnitCompanyType
    {
        Unit.Model UnitModel { get; }
        void SetSingularUnitType(Unit.Model unitModel);
    }
}