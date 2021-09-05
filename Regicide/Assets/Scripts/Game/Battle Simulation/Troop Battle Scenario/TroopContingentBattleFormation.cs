
using Regicide.Game.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    [RequireComponent(typeof(EntityColliderBrain))]
    [RequireComponent(typeof(TroopBattleRoster))]
    public class TroopContingentBattleFormation : BattleBehaviour, IEntityCollisionEnterObserver
    {
        private Dictionary<int, TroopBattleScenario> _troopContingentBattles = new Dictionary<int, TroopBattleScenario>();

        [SerializeField] private EntityColliderBrain _entityColliderBrain = null;
        [SerializeField] private TroopBattleFace[] _troopBattleFaces = null;
        [SerializeField] private TroopBattleRoster _troopBattleRoster = null;

        public TroopBattleRoster TroopBattleRoster { get => _troopBattleRoster; }

        public void OnEntityCollisionEnter(EntityCollision collision)
        {
            int hitEntityId = collision.HitEntityColliderBrain.Entity.EntityId;
            if (collision.IsCollidedEntitiesEnemies() && !_troopContingentBattles.ContainsKey(hitEntityId))
            {
                if (!_battleDamageableEntities.TryGetValue(hitEntityId, out BattleBehaviour enemyDamageableEntity))
                {
                    Debug.LogError("Enemy battle behaviour could not be found");
                    return;
                }

                if (!collision.ThisBattleCollider.TryGetComponent(out TroopBattleFace battleFace))
                {
                    Debug.LogError("Could not retrieve a Troop Battle Face");
                    return;
                }
                else 
                {
                    Func<IBattleLineDamageable<TroopUnitDamage>> battleLineDamageableResult = enemyDamageableEntity.GetBattleResult<IBattleLineDamageable<TroopUnitDamage>>(BattleId);
                    TroopBattleLine battleLine = battleFace.CreateTroopBattleLine();
                    TroopBattleScenario troopBattle = new TroopBattleScenario(this, battleLine);
                    _troopContingentBattles.Add(hitEntityId, troopBattle);
                    StartCoroutine(CommenceTroopContingentBattle(hitEntityId, battleLineDamageableResult));
                }
            }
        }

        public override Func<T> GetBattleResult<T>(int battleId)
        {
            return () =>
            {
                if (_troopContingentBattles.TryGetValue(battleId, out TroopBattleScenario troopContingentBattle) && troopContingentBattle.TroopBattleLine is T battleLine)
                {
                    return battleLine;
                }
                return default(T);
            };
        }

        private IEnumerator CommenceTroopContingentBattle(int enemyBattleId, Func<IBattleLineDamageable<TroopUnitDamage>> battleLineDamageablesResult)
        {
            yield return new WaitForEndOfFrame();
            IBattleLineDamageable<TroopUnitDamage> damageableBattleLine = battleLineDamageablesResult?.Invoke();
            if (damageableBattleLine != null && _troopContingentBattles.TryGetValue(enemyBattleId, out TroopBattleScenario battleScenario))
            {
                battleScenario.InitializeDamageableBattleLine(damageableBattleLine);
                battleScenario.StartBattle();
            }
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