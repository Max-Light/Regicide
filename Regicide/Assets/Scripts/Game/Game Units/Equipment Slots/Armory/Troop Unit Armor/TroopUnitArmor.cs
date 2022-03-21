
namespace Regicide.Game.Units
{
    public abstract class TroopUnitArmor : UnitEquipmentSlot
    {
        public virtual DamageReductionAttribute DamageReductionAttribute { get; } = DamageReductionAttribute.None;
    }
}