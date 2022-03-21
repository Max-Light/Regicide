
namespace Regicide.Game.Units
{
    public interface ITroopMeleeWeapon 
    {
        DamageAttribute DamageAttribute { get; }
        float MinRecoverTime { get; }
        float MaxRecoverTime { get; }
        float MinStrikeDelay { get; }
        float MaxStrikeDelay { get; }
        float ParryChance { get; }
        float GetRecoverTime();
        float GetStrikeDelayTime();
    }
}