
using System.Collections.Generic;

namespace Regicide.Game.BattleSimulation
{
    public interface IBattleLineDamageable<T> : IBattleLineDamageable
    {
        public IReadOnlyList<IBattleDamageable<T>> BattleLineDamageable { get; }
    }

    public interface IBattleLineDamageable : IBattleLineObserver { }
}