
namespace Regicide.Game.Units
{
    public interface ITroopUnit : IUnit
    {
        ITroopUnitHeadArmor HeadArmor { get; }
        ITroopUnitTorsoArmor TorsoArmor { get; }
        ITroopUnitLegArmor LegArmor { get; }
        ITroopUnitWeapon PrimaryWeapon { get; }
        ITroopUnitWeapon SecondaryWeapon { get; }
        ITroopUnitWeapon TertiaryWeapon { get; }
    }
}