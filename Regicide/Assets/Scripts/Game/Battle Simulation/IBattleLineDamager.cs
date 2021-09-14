
using System.Collections.Generic;

namespace Regicide.Game.BattleSimulation
{
    public interface IBattleLineDamager<T> : IBattleLineObserver, IBattlingEntity
    {
        public IReadOnlyList<IBattleDamager<T>> BattleLineDamager { get; }
    }
}