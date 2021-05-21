
namespace Regicide.Game.BattleSimulation
{
    public interface IFriendlyCommencer
    {
        void CommenceFriendlyCollision(BattleEntity thisBattleEntity, BattleEntity hitBattleEntity, BattleCollision battleCollision);
        void DecommenceFriendlyCollision(BattleEntity thisBattleEntity, BattleEntity hitBattleEntity, BattleCollision battleCollision);
    }
}