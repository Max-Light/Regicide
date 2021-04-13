using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Units
{
    public class MilitiaFootmanUnit : TroopUnit
    {
        public static UnitModel TroopModel { get; private set; } = new UnitModel
            (
            1,
            "Militia Footman",
            "A basic militia unit",
            null
            );

        public override UnitModel Model => TroopModel;
    }
}