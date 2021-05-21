
using System.Collections.Generic;

namespace Regicide.Game.BattleSimulation
{
    public interface IBattleLine 
    {
        int BattleID { get; }
        IDamager[] GetDamagers(BattleScenario battleScenario);
        IDamageable[] GetDamageables(BattleScenario battleScenario);
    }
}