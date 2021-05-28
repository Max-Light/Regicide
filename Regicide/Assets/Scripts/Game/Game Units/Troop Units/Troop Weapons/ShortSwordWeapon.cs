

namespace Regicide.Game.Units
{
    public class ShortSwordWeapon : UnitWeapon, ITroopUnitWeapon
    {
        public static Model ShortSwordModel { get; private set; } = new Model(
            1,
            "Short Sword",
            "A Short Sword",
            null
            );

        public static DamageReport DamageReport { get; private set; } = new DamageReport(
            25
            );

        public override Model WeaponModel => ShortSwordModel;
        public override DamageReport WeaponDamageReport => DamageReport;
    }
}