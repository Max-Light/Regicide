

namespace Regicide.Game.Units
{
    public abstract class TroopUnit : Unit, IDamager
    {
        public virtual ITroopUnitHeadArmor HeadArmor { get; private set; } = null;
        public virtual ITroopUnitTorsoArmor TorsoArmor { get; private set; } = null;
        public virtual ITroopUnitLegArmor LegArmor { get; private set; } = null;
        public virtual ITroopUnitWeapon PrimaryWeapon { get; private set; } = null;
        public virtual ITroopUnitWeapon SecondaryWeapon { get; private set; } = null;
        public virtual ITroopUnitWeapon TertiaryWeapon { get; private set; } = null;
        public ITroopUnitWeapon SelectedWeapon { get; private set; } = null;

        public virtual DamageReport DamageReport => SelectedWeapon.WeaponDamageReport;

        public TroopUnit()
        {
            SelectedWeapon = PrimaryWeapon;
        }
    }
}