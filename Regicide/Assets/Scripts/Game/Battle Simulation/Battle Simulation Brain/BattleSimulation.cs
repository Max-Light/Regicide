
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public static class BattleSimulation
    {
        private struct BattleKey
        {
            private int _battleEntityId_1;
            private int _battleEntityId_2;

            public BattleKey(IBattleLine thisBattleEntity, IBattleLine hitBattleEntity)
            {
                if (thisBattleEntity.BattleLineID < hitBattleEntity.BattleLineID)
                {
                    _battleEntityId_1 = thisBattleEntity.BattleLineID;
                    _battleEntityId_2 = hitBattleEntity.BattleLineID;
                }
                else
                {
                    _battleEntityId_1 = hitBattleEntity.BattleLineID;
                    _battleEntityId_2 = thisBattleEntity.BattleLineID;
                }
            }
        }

        private static Dictionary<BattleKey, BattleScenario> _battles = new Dictionary<BattleKey, BattleScenario>();
        public static IBattleUpdater BattleSimulationUpdater { get; private set; }

        public static bool TryGetBattleScenario(in IBattleLine battleLine_1, in IBattleLine battleLine_2, out BattleScenario battleScenario)
        {
            BattleKey key = new BattleKey(battleLine_1, battleLine_2);
            return _battles.TryGetValue(key, out battleScenario);
        }

        public static void CreateBattle(IBattleLine battleLine_1, IBattleLine battleLine_2)
        {
            BattleKey key = new BattleKey(battleLine_1, battleLine_2);
            if (!_battles.ContainsKey(key))
            {
                BattleScenario battleScenario = BattleScenarioFactory.GetBattleScenario(battleLine_1, battleLine_2);
                if (battleScenario != null)
                {
                    _battles.Add(key, battleScenario);
                    BattleSimulationUpdater?.StartBattle(battleScenario);
                }
            }
        }

        public static void StopBattle(IBattleLine battleLine_1, IBattleLine battleLine_2)
        {
            BattleKey key = new BattleKey(battleLine_1, battleLine_2);
            if (_battles.TryGetValue(key, out BattleScenario battleScenario))
            {
                BattleSimulationUpdater?.StopBattle(battleScenario);
                _battles.Remove(key);
            }
        }

        public static void SetBattleUpdater(IBattleUpdater battleSimulationUpdater)
        {
            if (BattleSimulationUpdater != null)
            {
                foreach (BattleScenario battleScenario in _battles.Values)
                {
                    BattleSimulationUpdater?.StopBattle(battleScenario);
                }
            }
            BattleSimulationUpdater = battleSimulationUpdater;
            if (BattleSimulationUpdater != null)
            {
                foreach (BattleScenario battleScenario in _battles.Values)
                {
                    battleSimulationUpdater.StartBattle(battleScenario);
                }
            }
        }
    }
}