using System;
using UnityEngine;

namespace Regicide.Game.Entity.BodyCollision
{
    public class NetworkEntityColliderBody : EntityColliderBody
    {
        private Action<BodyCollision> _entityServerCollisionEnterAction = null;
        private Action<BodyCollision> _entityServerCollisionExitAction = null;
        private Action<BodyCollision> _entityServerCollisionEnterFixedDelayAction = null;

        public void AddServerCollisionEnterObserver(Action<BodyCollision> collisionObserver) => _entityServerCollisionEnterAction += collisionObserver;
        public void RemoveServerCollisionEnterObserver(Action<BodyCollision> collisionObserver) => _entityServerCollisionEnterAction -= collisionObserver;
        public void AddServerCollisionExitObserver(Action<BodyCollision> collisionObserver) => _entityServerCollisionExitAction += collisionObserver;
        public void RemoveServerCollisionExitObserver(Action<BodyCollision> collisionObserver) => _entityServerCollisionExitAction -= collisionObserver;
        public void AddServerPrioritizedCollisionEnterObserver(Action<BodyCollision> collisionObserver) => _entityServerCollisionEnterFixedDelayAction += collisionObserver;
        public void RemoveServerPrioritizedCollisionEnterObserver(Action<BodyCollision> collisionObserver) => _entityServerCollisionEnterFixedDelayAction -= collisionObserver;

        protected override void OnBodyCollisionEnter(BodyCollision bodyCollision)
        {
            if (isServer)
            {
                _entityServerCollisionEnterAction?.Invoke(bodyCollision);
            }
            else if (isClient)
            {
                base.OnBodyCollisionEnter(bodyCollision);
            }
        }

        protected override void OnBodyCollisionExit(BodyCollision bodyCollision)
        {
            if (isServer)
            {
                _entityServerCollisionExitAction?.Invoke(bodyCollision);
            }
            else if (isClient)
            {
                base.OnBodyCollisionExit(bodyCollision);
            }
        }

        protected override void OnBodyPrioritizedCollsionEnter(BodyCollision bodyCollision)
        {
            if (isServer)
            {
                _entityServerCollisionEnterFixedDelayAction?.Invoke(bodyCollision);
            }
            else if (isClient)
            {
                base.OnBodyPrioritizedCollsionEnter(bodyCollision);
            }
        }
    }
}