
namespace Regicide.Game.Units
{
    public interface ITroopOneHandedMeleeWeapon : ITroopMeleeWeapon
    {
        ITroopShieldWeapon ShieldWeapon { get; }
        bool HasShieldEquipped();
        void EquipShield(ITroopShieldWeapon shield);
        void UnequipShield();
    }
}