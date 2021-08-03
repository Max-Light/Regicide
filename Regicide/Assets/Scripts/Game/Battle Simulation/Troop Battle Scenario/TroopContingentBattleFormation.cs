
using Regicide.Game.EntityCollision;
using Regicide.Game.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    [RequireComponent(typeof(EntityColliderBrain))]
    [RequireComponent(typeof(TroopBattleRoster))]
    public class TroopContingentBattleFormation : BattleObject, IEntityCollisionObserver
    {
        private Dictionary<int, TroopBattleScenario> _troopContingentBattles = new Dictionary<int, TroopBattleScenario>();

        [SerializeField] private EntityColliderBrain _entityColliderBrain = null;
        [SerializeField] private TroopBattleFace[] _troopBattleFaces = null;
        [SerializeField] private TroopBattleRoster _troopBattleRoster = null;

        public TroopBattleRoster TroopBattleRoster { get => _troopBattleRoster; }

        public void OnEntityCollisionEnter(EntityColliderBrain thisEntity, EntityColliderBrain hitEntity, EntityCollision.EntityCollision collision)
        {
            if (thisEntity.Entity.IsEnemy(hitEntity.Entity) && _battleDamageableEntities.TryGetValue(hitEntity.EntityId, out BattleObject enemyDamageableEntity))
            {
                int enemyBattleId = hitEntity.EntityId;
                if (!_troopContingentBattles.ContainsKey(enemyBattleId) 
                    && enemyDamageableEntity.TryGetBattleLineResult(BattleId, out Func<IReadOnlyList<IBattleDamageable<TroopUnitDamage>>> battleLineResult)
                    && collision.ThisBattleCollider.TryGetComponent(out TroopBattleFace battleFace) 
                    )
                {
                    TroopBattleLine battleLine = battleFace.CreateTroopBattleLine();
                    TroopBattleScenario troopBattle = new TroopBattleScenario(this, battleLine);
                    _troopContingentBattles.Add(enemyBattleId, troopBattle);
                    StartCoroutine(CommenceTroopContingentBattle(enemyBattleId, battleLineResult));
                }
            }
        }

        public void OnEntityCollisionExit(EntityColliderBrain thisEntity, EntityColliderBrain hitEntity)
        {
            
        }

        public override bool TryGetBattleLineResult<T>(int battleId, out Func<IReadOnlyList<T>> battleLineResult)
        {
            if (TroopBattleLine.ContainsBattleUnitOfType<T>())
            {
                battleLineResult = () =>
                {
                    if (_troopContingentBattles.TryGetValue(battleId, out TroopBattleScenario troopContingentBattle))
                    {
                        return (IReadOnlyList<T>)troopContingentBattle.TroopBattleLine.Cast<T>();
                    }
                    return null;
                };
                return true;
            }
            battleLineResult = null;
            return false;
        }

        public override bool TryGetBattleLineObserver(int battleId, out IObservable observer)
        {
            if (_troopContingentBattles.TryGetValue(battleId, out TroopBattleScenario troopContingentBattle))
            {
                observer = troopContingentBattle.TroopBattleLine;
                return true;
            }
            observer = null;
            return false;
        }

        private IEnumerator CommenceTroopContingentBattle(int enemyBattleId, Func<IReadOnlyList<IBattleDamageable<TroopUnitDamage>>> battleLineDamageables)
        {
            yield return new WaitForEndOfFrame();
            IReadOnlyList<IBattleDamageable<TroopUnitDamage>> damageables = battleLineDamageables?.Invoke();
            if (damageables != null && _battleDamageableEntities.TryGetValue(enemyBattleId, out BattleObject battleFormation) && battleFormation.TryGetBattleLineObserver(BattleId, out IObservable observer))
            {
                if (_troopContingentBattles.TryGetValue(enemyBattleId, out TroopBattleScenario troopUnitLineBattle))
                {
                    troopUnitLineBattle.InitializeDamageableBattleLine(damageables, observer);
                    troopUnitLineBattle.StartBattle();
                }
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