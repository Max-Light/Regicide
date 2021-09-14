
using Regicide.Game.Entity;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    [RequireComponent(typeof(EntityColliderBrain))]
    public class TroopContingentBattleSimulator : BattleSimulator, IEntityCollisionEnterObserver
    {
        [SerializeField] private EntityColliderBrain _entityColliderBrain = null;
        [SerializeField] private TroopBattleRoster _troopBattleRoster = null;

        public TroopBattleRoster TroopBattleRoster { get => _troopBattleRoster; }

        public void OnEntityCollisionEnter(EntityCollision collision)
        {
            int hitEntityId = collision.HitEntityColliderBrain.Entity.EntityId;
            if (collision.IsCollidedEntitiesEnemies() && !_battleScenarios.ContainsKey(hitEntityId))
            {
                if (!_battleSimulators.TryGetValue(hitEntityId, out BattleSimulator enemyBattleBehaviour))
                {
                    Debug.LogError("Enemy battle behaviour could not be found");
                    return;
                }

                if (!collision.ThisBattleCollider.TryGetComponent(out TroopBattleFace battleFace))
                {
                    Debug.LogError("Could not retrieve a Troop Battle Face");
                    return;
                }

                if (enemyBattleBehaviour.TryGetBattleScenario(BattleId, out BattleScenario enemyBattleScenario))
                {
                    if (enemyBattleScenario is TroopBattleScenario enemyTroopBattleScenario && enemyTroopBattleScenario.DamageableBattleLine is TroopBattleLine troopBattleLine)
                    {
                        TroopBattleScenario troopBattleScenario = new TroopBattleScenario(this, troopBattleLine);
                        troopBattleScenario.InitializeDamageableBattleLine(enemyTroopBattleScenario.TroopBattleLine);
                        _battleScenarios.Add(hitEntityId, troopBattleScenario);
                        StartCoroutine(CommenceBattleScenarioAtEndOfFrame(troopBattleScenario));
                    }
                }
                else if (enemyBattleBehaviour.TryGetBattlingEntity(collision.HitBattleCollider.transform, out IBattleLineDamageable<TroopUnitDamage> troopBattleLineDamageable))
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
            _entityColliderBrain = GetComponent<EntityColliderBrain>();
            _troopBattleRoster = GetComponent<TroopBattleRoster>();
        }

        private void OnEnable()
        {
            _entityColliderBrain.AddEntityCollisionObserver(this);
        }

        private void OnDisable()
        {
            _entityColliderBrain.RemoveEntityCollisionObserver(this);
        }
    }
}