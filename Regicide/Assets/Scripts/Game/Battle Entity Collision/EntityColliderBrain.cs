using Mirror;
using Regicide.Game.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.EntityCollision
{
    [RequireComponent(typeof(IEntity))]
    public class EntityColliderBrain : NetworkBehaviour
    {
        [SerializeField] private IEntity _entity = null;
        [SerializeField] protected EntityCollider[] _entityColliders = null;
        protected Dictionary<EntityColliderBrain, EntityCollision> _collidedEntities = new Dictionary<EntityColliderBrain, EntityCollision>();
        private HashSet<IEntityCollisionObserver> _entityCollisionObservers = new HashSet<IEntityCollisionObserver>();

        public int EntityId { get => _entity.EntityId; }
        public IEntity Entity { get => _entity; }
        public EntityCollider[] EntityColliders { get => _entityColliders; }

        public void AddEntityCollisionObserver(IEntityCollisionObserver collisionObserver) => _entityCollisionObservers.Add(collisionObserver);
        public void RemoveEntityCollisionObserver(IEntityCollisionObserver collisionObserver) => _entityCollisionObservers.Remove(collisionObserver);

        protected virtual void OnCollisionEnter(Collision collision)
        {
            ContactPoint contact = collision.GetContact(0);
            if (collision.rigidbody.TryGetComponent(out EntityColliderBrain hitEntity) && contact.thisCollider.TryGetComponent(out EntityCollider thisEntityCollider) && contact.otherCollider.TryGetComponent(out EntityCollider hitEntityCollider))
            {
                OnEntityCollisionEnter(hitEntity, thisEntityCollider, hitEntityCollider);
            }
        }

        protected void OnEntityCollisionEnter(EntityColliderBrain hitBattleEntity, EntityCollider thisBattleCollider, EntityCollider hitBattleCollider)
        {
            if (!_collidedEntities.ContainsKey(hitBattleEntity))
            {
                _collidedEntities.Add(hitBattleEntity, new EntityCollision(thisBattleCollider, hitBattleCollider));
                StartCoroutine(TriggerEntityCollisionEnter(hitBattleEntity));
            }
            else
            {
                if (_collidedEntities[hitBattleEntity].HitBattleCollider.CollisionPriority < hitBattleCollider.CollisionPriority)
                {
                    _collidedEntities[hitBattleEntity] = new EntityCollision(thisBattleCollider, hitBattleCollider);
                }
            }
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if (collision.rigidbody.TryGetComponent(out EntityColliderBrain hitBattleEntity))
            {
                TriggerEntityCollisionExit(hitBattleEntity);
            }
        }

        protected void TriggerEntityCollisionExit(EntityColliderBrain hitEntity)
        {
            foreach (IEntityCollisionObserver colliderObserver in _entityCollisionObservers)
            {
                colliderObserver.OnEntityCollisionExit(this, hitEntity);
            }
        }

        private IEnumerator TriggerEntityCollisionEnter(EntityColliderBrain hitEntity)
        {
            yield return new WaitForFixedUpdate();
            foreach (IEntityCollisionObserver colliderObserver in _entityCollisionObservers)
            {
                colliderObserver.OnEntityCollisionEnter(this, hitEntity, _collidedEntities[hitEntity]);
            }
            _collidedEntities.Remove(hitEntity);
        }

        private void Awake()
        {
            _entity = GetComponent<IEntity>();
        }
    }
}