using System;
using UnityEngine;

namespace Regicide.Game.Units
{
    public abstract class TroopUnit : Unit
    {
        public ITroopUnitHeadArmor HeadArmor { get; private set; } = null;
        public ITroopUnitTorsoArmor TorsoArmor { get; private set; } = null;
        public ITroopUnitLegArmor LegArmor { get; private set; } = null;
        public ITroopUnitWeapon PrimaryWeapon { get; private set; } = null;
        public ITroopUnitWeapon SecondaryWeapon { get; private set; } = null;
        public ITroopUnitWeapon TertiaryWeapon { get; private set; } = null;

        public override void TakeDamage(float damage)
        {
            _onUnitChange?.Invoke();
        }
    }
}