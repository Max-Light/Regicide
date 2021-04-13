using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Units
{
    public abstract class UnitArmorPiece : IUnitArmor
    {
        public virtual UnitArmorModel Model => null;
    }
}