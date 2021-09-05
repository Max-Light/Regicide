
using System.Collections.Generic;

namespace Regicide.Game.BattleSimulation
{
    public interface IBattleLineDamager<T> : IBattleLineDamager
    {
        public IReadOnlyList<IBattleDamager<T>> BattleLineDamager { get; }
    }

    public interface IBattleLineDamager : IBattleLineObserver { }
}