
namespace Regicide.Game.BattleSimulation
{
    public interface IBattleDamageable<T> : IBattleDamageable
    {
        bool ReceiveDamage(T damageInfliction);
    }

    public interface IBattleDamageable : IBattleUnit, IBattlingEntity { }
}