
namespace Regicide.Game.Units
{
    public class MilitiaFootmanUnit : TroopUnit
    {
        private ITroopUnitWeapon _unitWeapon = new ShortSwordWeapon();

        public static Model TroopModel { get; private set; } = new Model(
            1,
            "Militia Footman",
            "A basic militia unit",
            null
            );

        public override Model UnitModel => TroopModel;

        public override ITroopUnitWeapon PrimaryWeapon { get => _unitWeapon; }
        public override DamageReport DamageReport => _unitWeapon.WeaponDamageReport;
    }
}