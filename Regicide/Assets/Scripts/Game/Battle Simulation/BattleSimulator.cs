using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public abstract class BattleSimulator : NetworkBehaviour
    {
        protected static Dictionary<int, BattleSimulator> _battleSimulators = new Dictionary<int, BattleSimulator>();
        protected Dictionary<int, BattleScenario> _battleScenarios = new Dictionary<int, BattleScenario>();

        public int BattleId { get => (int)netId; }

        public virtual bool TryGetBattlingEntity<T>(out T battlingEntity) where T : IBattlingEntity { battlingEntity = default(T); return false; }
        public virtual bool TryGetBattlingEntity<T>(Transform transform, out T battlingEntity) where T : IBattlingEntity { battlingEntity = default(T); return false; }
        public bool TryGetBattleScenario(int battleId, out BattleScenario battleScenario) { return _battleScenarios.TryGetValue(battleId, out battleScenario); }
        public bool IsBattlingBattleBehaviour(int battleId) { return _battleScenarios.ContainsKey(battleId); }
        public bool IsBattlingBattleBehaviour(BattleSimulator battleBehaviour) { return _battleScenarios.ContainsKey(battleBehaviour.BattleId); }
        public bool IsBattling() { return _battleScenarios.Count > 0; }

        public override void OnStartClient()
        {
            if (!_battleSimulators.ContainsKey(BattleId))
            {
                _battleSimulators.Add(BattleId, this);
            }
            else
            {
                Debug.LogError("Battle Behaviour already exists on " + gameObject.name + ". There can only be one Battle Behaviour object for each gameobject.");
            }
        }

        protected IEnumerator CommenceBattleScenarioAtEndOfFrame(BattleScenario battleScenario)
        {
            yield return new WaitForEndOfFrame();
            battleScenario.StartBattle();
        }

        private void OnDestroy()
        {
            if (_battleSimulators.TryGetValue(BattleId, out BattleSimulator battleBehaviour) && battleBehaviour == this)
            {
                _battleSimulators.Remove(BattleId);
            }
        }
    }
}