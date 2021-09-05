using Mirror;
using Regicide.Game.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Entity
{
    [RequireComponent(typeof(IEntity))]
    public class EntityColliderBrain : NetworkBehaviour
    {
        [SerializeField] protected IEntity _entity = null;
        [SerializeField] protected EntitySubcollider[] _entityColliders = null;

        private Dictionary<EntityColliderBrain, EntityCollision> _collidedEntitiesInFrame = new Dictionary<EntityColliderBrain, EntityCollision>();
        private HashSet<IEntityCollisionEnterObserver> _entityCollisionEnterObservers = new HashSet<IEntityCollisionEnterObserver>();
        private HashSet<IEntityCollisionExitObserver> _entityCollisionExitObservers = new HashSet<IEntityCollisionExitObserver>();

        public IEntity Entity { get => _entity; }
        public EntitySubcollider[] EntityColliders { get => _entityColliders; }

        public void AddEntityCollisionObserver(IEntityCollisionEnterObserver collisionObserver) => _entityCollisionEnterObservers.Add(collisionObserver);
        public void AddEntityCollisionObserver(IEntityCollisionExitObserver collisionObserver) => _entityCollisionExitObservers.Add(collisionObserver);
        public void RemoveEntityCollisionObserver(IEntityCollisionEnterObserver collisionObserver) => _entityCollisionEnterObservers.Remove(collisionObserver);
        public void RemoveEntityCollisionObserver(IEntityCollisionExitObserver collisionObserver) => _entityCollisionExitObservers.Remove(collisionObserver);

        protected virtual void OnCollisionEnter(Collision collision)
        {
            ContactPoint contact = collision.GetContact(0);
            if (collision.rigidbody != null 
                && collision.rigidbody.TryGetComponent(out EntityColliderBrain hitEntity) 
                && contact.thisCollider.TryGetComponent(out EntitySubcollider thisEntityCollider) 
                && contact.otherCollider.TryGetComponent(out EntitySubcollider hitEntityCollider))
            {
                OnEntityCollisionEnter(hitEntity, thisEntityCollider, hitEntityCollider);
            }
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if (collision.rigidbody != null 
                && collision.rigidbody.TryGetComponent(out EntityColliderBrain hitBattleEntity))
            {
                TriggerEntityCollisionExit(hitBattleEntity);
            }
        }

        private void OnEntityCollisionEnter(EntityColliderBrain hitEntity, EntitySubcollider thisBattleCollider, EntitySubcollider hitBattleCollider)
        {
            if (!_collidedEntitiesInFrame.ContainsKey(hitEntity))
            {
                _collidedEntitiesInFrame.Add(hitEntity, new EntityCollision(this, hitEntity, thisBattleCollider, hitBattleCollider));
                StartCoroutine(TriggerEntityCollisionEnter(hitEntity));
            }
            else
            {
                if (_collidedEntitiesInFrame[hitEntity].HitBattleCollider.CollisionPriority < hitBattleCollider.CollisionPriority)
                {
                    _collidedEntitiesInFrame[hitEntity] = new EntityCollision(this, hitEntity, thisBattleCollider, hitBattleCollider);
                }
            }
        }

        private IEnumerator TriggerEntityCollisionEnter(EntityColliderBrain hitEntity)
        {
            yield return new WaitForFixedUpdate();
            foreach (IEntityCollisionEnterObserver colliderObserver in _entityCollisionEnterObservers)
            {
                colliderObserver.OnEntityCollisionEnter(_collidedEntitiesInFrame[hitEntity]);
            }
            _collidedEntitiesInFrame.Remove(hitEntity);
        }

        private void TriggerEntityCollisionExit(EntityColliderBrain hitEntity)
        {
            foreach (IEntityCollisionExitObserver colliderObserver in _entityCollisionExitObservers)
            {
                colliderObserver.OnEntityCollisionExit(this, hitEntity);
            }
        }

        private void Awake()
        {
            _entity = GetComponent<IEntity>();
            if (_entity == null)
            {
                Debug.LogError("Entity Collider Brain needs to be attached to entity!");
            }
        }
    }
}