using Mirror;
using Regicide.Game.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Entity.BodyCollision
{
    [RequireComponent(typeof(IEntity))]
    public class EntityColliderBody : NetworkBehaviour
    {
        [SerializeField] protected IEntity _entity = null;

        private Action<BodyCollision> _entityCollisionEnterAction = null;
        private Action<BodyCollision> _entityCollisionExitAction = null;
        private Action<BodyCollision> _entityCollisionEnterFixedDelayAction = null;
        private Dictionary<EntityColliderBody, BodyCollision> _triggeredEnterCollisionsInUpdate = new Dictionary<EntityColliderBody, BodyCollision>();

        public IEntity Entity { get => _entity; }

        public void AddCollisionEnterObserver(Action<BodyCollision> collisionObserver) => _entityCollisionEnterAction += collisionObserver;
        public void RemoveCollisionEnterObserver(Action<BodyCollision> collisionObserver) => _entityCollisionEnterAction -= collisionObserver;
        public void AddCollisionExitObserver(Action<BodyCollision> collisionObserver) => _entityCollisionExitAction += collisionObserver;
        public void RemoveCollisionExitObserver(Action<BodyCollision> collisionObserver) => _entityCollisionExitAction -= collisionObserver;
        public void AddPrioritizedCollisionEnterObserver(Action<BodyCollision> collisionObserver) => _entityCollisionEnterFixedDelayAction += collisionObserver;
        public void RemovePrioritizedCollisionEnterObserver(Action<BodyCollision> collisionObserver) => _entityCollisionEnterFixedDelayAction -= collisionObserver;

        private void OnCollisionEnter(Collision collision)
        {
            EntityColliderBody hitBody;
            if ((collision.rigidbody != null && collision.rigidbody.TryGetComponent(out hitBody)) || collision.collider.TryGetComponent(out hitBody))
            {
                BodyCollision bodyCollision = new BodyCollision(this, hitBody, collision);
                OnBodyCollisionEnter(bodyCollision);
                TriggerBodyCollisionFixedDelay(bodyCollision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            EntityColliderBody hitBody;
            if ((collision.rigidbody != null && collision.rigidbody.TryGetComponent(out hitBody)) || collision.collider.TryGetComponent(out hitBody))
            {
                OnBodyCollisionExit(new BodyCollision(this, hitBody, collision.collider));
            }
        }

        protected virtual void OnBodyCollisionEnter(BodyCollision bodyCollision)
        {
            _entityCollisionEnterAction?.Invoke(bodyCollision);
        }

        protected virtual void OnBodyCollisionExit(BodyCollision bodyCollision)
        {
            _entityCollisionExitAction?.Invoke(bodyCollision);
        }

        protected virtual void OnBodyPrioritizedCollsionEnter(BodyCollision bodyCollision)
        {
            _entityCollisionEnterFixedDelayAction?.Invoke(bodyCollision);
        }
        
        private void TriggerBodyCollisionFixedDelay(BodyCollision bodyCollision)
        {
            EntityColliderBody hitBody = bodyCollision.HitEntityColliderBody;
            if (!_triggeredEnterCollisionsInUpdate.ContainsKey(hitBody))
            {
                _triggeredEnterCollisionsInUpdate.Add(hitBody, bodyCollision);
                StartCoroutine(WaitForPrioritizedEnterCollision(hitBody));
            }
            else if (_triggeredEnterCollisionsInUpdate[hitBody].HitCollider.TryGetComponent(out SubcolliderPriorityAttributer originalSubcollider)
                && bodyCollision.HitCollider.TryGetComponent(out SubcolliderPriorityAttributer subcollider) 
                && originalSubcollider.CollisionPriority > subcollider.CollisionPriority)
            {
                _triggeredEnterCollisionsInUpdate[hitBody] = bodyCollision;
            }
        }

        private IEnumerator WaitForPrioritizedEnterCollision(EntityColliderBody hitBody)
        {
            yield return new WaitForFixedUpdate();
            OnBodyPrioritizedCollsionEnter(_triggeredEnterCollisionsInUpdate[hitBody]);
            _triggeredEnterCollisionsInUpdate.Remove(hitBody);
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