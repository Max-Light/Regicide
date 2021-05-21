
using System;

namespace Regicide.Game.BattleSimulation
{
    public class BattleScenarioBuilder
    {
        private Type _battleScenarioType = null;
        private IBattleLine _battleCommencer_1 = null, _battleCommencer_2 = null;

        public BattleScenarioBuilder(Type battleScenarioType, IBattleLine battleCommencer_1, IBattleLine battleCommencer_2)
        {
            _battleScenarioType = battleScenarioType;
            _battleCommencer_1 = battleCommencer_1;
            _battleCommencer_2 = battleCommencer_2;
        }

        public BattleScenario Build()
        {
            if (_battleScenarioType == null)
            {
                return null;
            }
            Object[] parameters = { _battleCommencer_1, _battleCommencer_2 };
            return Activator.CreateInstance(_battleScenarioType, parameters) as BattleScenario;
        }

        public static implicit operator BattleScenario(BattleScenarioBuilder builder)
        {
            return builder.Build();
        }
    }
}