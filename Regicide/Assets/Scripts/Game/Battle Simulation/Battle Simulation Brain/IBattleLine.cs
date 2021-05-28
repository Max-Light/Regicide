
using Regicide.Game.Units;
using System.Collections.Generic;

namespace Regicide.Game.BattleSimulation
{
    public interface IBattleLine 
    {
        int BattleLineID { get; }
        IReadOnlyList<IDamager> GetDamagers(BattleScenario battleScenario);
        IReadOnlyList<IDamageable> GetDamageables(BattleScenario battleScenario);
    }
}