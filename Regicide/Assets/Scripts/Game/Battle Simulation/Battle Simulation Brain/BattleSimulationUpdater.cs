
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public sealed class BattleSimulationUpdater : NetworkBehaviour, IBattleUpdater
    {
        private Dictionary<BattleScenario, IEnumerator> _runningBattleRoutines = new Dictionary<BattleScenario, IEnumerator>();

        public void StartBattle(BattleScenario battleScenario)
        {
            IEnumerator battleUpdate = BattleUpdate(battleScenario);
            _runningBattleRoutines.Add(battleScenario, battleUpdate);
            StartCoroutine(battleUpdate);
        }

        public void StopBattle(BattleScenario battleScenario)
        {
            if (_runningBattleRoutines.TryGetValue(battleScenario, out IEnumerator battleUpdate))
            {
                StopCoroutine(battleUpdate);
            }
        }

        private IEnumerator BattleUpdate(BattleScenario battleScenario)
        {
            yield return new WaitForEndOfFrame();
            while (true)
            {
                battleScenario.UpdateBattleInflictions();
                if (!battleScenario.IsBattling)
                {
                    break;
                }
                yield return new WaitForSeconds(battleScenario.BattleCycleTimeLength);
            }
            _runningBattleRoutines.Remove(battleScenario);
        }
        
        private void Awake()
        {
            BattleSimulation.SetBattleUpdater(this);
            BattleScenarioFactory.InitializeFactory();
        }

        private void Start()
        {
            if (BattleSimulation.BattleSimulationUpdater is BattleSimulationUpdater battleSimulationUpdater && battleSimulationUpdater != this)
            {
                Debug.Log("Detected numerous Battle Simulation Updaters. Destroying superfluous updater.");
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            BattleScenarioFactory.ClearFactory();
            if (BattleSimulation.BattleSimulationUpdater.Equals(this))
            {
                BattleSimulation.SetBattleUpdater(null);
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isServer) { return; }
            Destroy(gameObject);
        }
    }
}