
namespace Regicide.Game.BattleSimulation
{
    public interface IBattleCommencer : IBattleLine
    {
        void CommenceBattle(BattleEntity thisBattleEntity, BattleEntity hitBattleEntity, BattleCollision battleCollision);
        void DecommenceBattle(BattleEntity thisBattleEntity, BattleEntity hitBattleEntity, BattleCollision battleCollision);
    }
}