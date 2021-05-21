using Mirror;
using Regicide.Game.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class BattleEntity : NetworkBehaviour
    {
        [SerializeField] protected IOwnableEntity _entity = null;
        [SerializeField] protected IBattleCommencer _battleCommencer = null;
        [SerializeField] protected IFriendlyCommencer _friendlyCommencer = null;
        [SerializeField] protected BattleCollider[] _battleColliders = null;
        protected Dictionary<BattleEntity, BattleCollision> _collidedEntities = new Dictionary<BattleEntity, BattleCollision>();

        public IBattleCommencer BattleCommencer { get => _battleCommencer; }
        public BattleCollider[] BattleColliders { get => _battleColliders; }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            ContactPoint contact = collision.GetContact(0);
            if (collision.rigidbody.TryGetComponent(out BattleEntity hitBattleEntity) && contact.thisCollider.TryGetComponent(out BattleCollider thisBattleCollider) && contact.otherCollider.TryGetComponent(out BattleCollider hitBattleCollider))
            {
                OnBattleEntityCollisionEnter(hitBattleEntity, thisBattleCollider, hitBattleCollider);
            }
        }

        protected virtual void OnBattleEntityCollisionEnter(BattleEntity hitBattleEntity, BattleCollider thisBattleCollider, BattleCollider hitBattleCollider)
        {
            if (!_collidedEntities.ContainsKey(hitBattleEntity))
            {
                _collidedEntities.Add(hitBattleEntity, new BattleCollision(thisBattleCollider, hitBattleCollider));
                StartCoroutine(CommenceBattleEntityCollision(hitBattleEntity));
            }
            else
            {
                if (_collidedEntities[hitBattleEntity].HitBattleCollider.CollisionPriority < hitBattleCollider.CollisionPriority)
                {
                    _collidedEntities[hitBattleEntity] = new BattleCollision(thisBattleCollider, hitBattleCollider);
                }
            }
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if (collision.rigidbody.TryGetComponent(out BattleEntity hitBattleEntity))
            {
                OnBattleEntityCollisionExit(hitBattleEntity);
            }
        }

        protected virtual void OnBattleEntityCollisionExit(BattleEntity hitBattleEntity)
        {
            _collidedEntities.Remove(hitBattleEntity);
        }

        private IEnumerator CommenceBattleEntityCollision(BattleEntity hitEntity)
        {
            yield return new WaitForFixedUpdate();
            if (_entity != null)
            {
                if (_entity.IsFriendly(hitEntity._entity))
                {
                    _friendlyCommencer?.CommenceFriendlyCollision(this, hitEntity, _collidedEntities[hitEntity]);
                }
                else
                {
                    _battleCommencer?.CommenceBattle(this, hitEntity, _collidedEntities[hitEntity]);
                }
            }
            else
            {
                Debug.Log("Missing ownable entity reference.");
            }
        }

        private void OnValidate()
        {
            _entity = GetComponent<IOwnableEntity>();
            _battleCommencer = GetComponent<IBattleCommencer>();
            _friendlyCommencer = GetComponent<IFriendlyCommencer>();
        }
    }
}