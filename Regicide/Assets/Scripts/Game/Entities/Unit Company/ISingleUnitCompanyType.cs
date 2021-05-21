using Regicide.Game.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Entities
{
    public interface ISingleUnitCompanyType
    {
        Type UnitType { get; }
        void SetSingularUnitType<T>() where T : Unit;
    }
}