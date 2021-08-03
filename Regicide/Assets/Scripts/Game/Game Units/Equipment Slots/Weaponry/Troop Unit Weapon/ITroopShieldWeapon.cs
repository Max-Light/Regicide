
namespace Regicide.Game.Units
{
    public interface ITroopShieldWeapon 
    {
        float BlockPercentage { get; }
        float HitPoints { get; }
        float RecoverTimePenalty { get; }
        float StrikeDelayPenalty { get; }
    }
}