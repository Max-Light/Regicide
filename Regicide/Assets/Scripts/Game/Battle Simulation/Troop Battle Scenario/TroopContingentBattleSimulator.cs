
using Regicide.Game.Entity.BodyCollision;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    [RequireComponent(typeof(EntityColliderBody))]
    public class TroopContingentBattleSimulator : BattleSimulator
    {
        [SerializeField] private NetworkEntityColliderBody _entityColliderBody = null;
        [SerializeField] private TroopBattleRoster _troopBattleRoster = null;

        public TroopBattleRoster TroopBattleRoster { get => _troopBattleRoster; }

        public void OnEntityCollisionEnter(BodyCollision bodyCollision)
        {
            int hitEntityId = bodyCollision.HitEntityColliderBody.Entity.EntityId;
            if (bodyCollision.IsEnemyCollision() && !_battleScenarios.ContainsKey(hitEntityId))
            {
                if (!_battleSimulators.TryGetValue(hitEntityId, out BattleSimulator enemyBattleSimulator))
                {
                    Debug.LogError("Enemy battle behaviour could not be found");
                    return;
                }

                if (!bodyCollision.ThisCollider.TryGetComponent(out TroopBattleFace battleFace))
                {
                    Debug.LogError("Could not retrieve a Troop Battle Face");
                    return;
                }

                if (enemyBattleSimulator.TryGetBattleScenario(BattleId, out BattleScenario enemyBattleScenario))
                {
                    if (enemyBattleScenario is TroopBattleScenario enemyTroopBattleScenario && enemyTroopBattleScenario.DamageableBattleLine is TroopBattleLine troopBattleLine)
                    {
                        TroopBattleScenario troopBattleScenario = new TroopBattleScenario(this, troopBattleLine);
                        troopBattleScenario.InitializeDamageableBattleLine(enemyTroopBattleScenario.TroopBattleLine);
                        _battleScenarios.Add(hitEntityId, troopBattleScenario);
                        StartCoroutine(CommenceBattleScenarioAtEndOfFrame(troopBattleScenario));
                    }
                }
                else if (enemyBattleSimulator.TryGetBattlingEntity(bodyCollision.HitCollider.transform, out IBattleLineDamageable<TroopUnitDamage> troopBattleLineDamageable))
                {
                    TroopBattleLine troopBattleLine = battleFace.CreateTroopBattleLine();
                    TroopBattleScenario troopBattleScenario = new TroopBattleScenario(this, troopBattleLine);
                    troopBattleScenario.InitializeDamageableBattleLine(troopBattleLineDamageable);
                    _battleScenarios.Add(hitEntityId, troopBattleScenario);
                    StartCoroutine(CommenceBattleScenarioAtEndOfFrame(troopBattleScenario));
                }
            }
        }

        public override bool TryGetBattlingEntity<T>(Transform transform, out T battlingEntity)
        {
            if (typeof(T).IsAssignableFrom(typeof(TroopBattleLine)) && transform.TryGetComponent(out TroopBattleFace troopBattleFace))
            {
                if (troopBattleFace.CreateTroopBattleLine() is T troopBattleLine)
                {
                    battlingEntity = troopBattleLine;
                    return true;
                }
            }
            battlingEntity = default(T);
            return false;
        }

        private void OnValidate()
        {
            _entityColliderBody = GetComponent<NetworkEntityColliderBody>();
            _troopBattleRoster = GetComponent<TroopBattleRoster>();
        }

        protected virtual void OnEnable()
        {
            _entityColliderBody.AddServerPrioritizedCollisionEnterObserver(OnEntityCollisionEnter);
        }

        protected virtual void OnDisable()
        {
            _entityColliderBody.RemoveServerPrioritizedCollisionEnterObserver(OnEntityCollisionEnter);
        }
    }
}