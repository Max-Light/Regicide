
using System.Collections;

namespace Regicide.Game.BattleSimulation
{
    public interface IBattleDamager<T> : IBattleDamager
    {
        IEnumerator CommenceBattleFighting(IBattleDamageable<T> damageable);
    }

    public interface IBattleDamager : IBattleUnit, IBattlingEntity { }
}