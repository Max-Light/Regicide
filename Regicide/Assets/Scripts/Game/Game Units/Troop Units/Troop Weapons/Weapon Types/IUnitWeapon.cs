
namespace Regicide.Game.Units
{
    public interface IUnitWeapon
    {
        UnitWeapon.Model WeaponModel { get; }
        DamageReport WeaponDamageReport { get; }
    }
}