
namespace Regicide.Game.BattleSimulation
{
    public class BattleCollision 
    {
        public BattleCollider ThisBattleCollider { get; private set; } = null;
        public BattleCollider HitBattleCollider { get; private set; } = null;

        public BattleCollision(BattleCollider thisBattleCollider, BattleCollider hitBattleCollider)
        {
            ThisBattleCollider = thisBattleCollider;
            HitBattleCollider = hitBattleCollider;
        }
    }
}