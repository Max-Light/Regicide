
using System.Collections.Generic;

namespace Regicide.Game.BattleSimulation
{
    public interface IBattleLineDamageable<T> : IBattleLineObserver, IBattlingEntity
    {
        public IReadOnlyList<IBattleDamageable<T>> BattleLineDamageable { get; }
    }
}