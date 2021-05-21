
using Regicide.Game.BattleSimulation;

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

        public override void PopulateDamageReport(DamageReport damageReport)
        {
            if (damageReport is TroopUnitDamage troopUnitDamage)
            {

            }
        }

        public override void ReceiveDamage(DamageReport damage)
        {
            if (damage is TroopUnitDamage troopDamage)
            {

            }
        }
    }
}