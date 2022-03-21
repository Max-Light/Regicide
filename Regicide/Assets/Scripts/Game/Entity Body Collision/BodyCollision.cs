
using UnityEngine;

namespace Regicide.Game.Entity.BodyCollision
{
    public struct BodyCollision 
    {
        private EntityColliderBody _thisEntityColliderBody;
        private EntityColliderBody _hitEntityColliderBody;
        private Collider _thisCollider;
        private Collider _hitCollider;

        public EntityColliderBody ThisEntityColliderBody { get => _thisEntityColliderBody; }
        public EntityColliderBody HitEntityColliderBody { get => _hitEntityColliderBody; }
        public Collider ThisCollider { get => _thisCollider; }
        public Collider HitCollider { get => _hitCollider; }

        public BodyCollision(EntityColliderBody thisBody, EntityColliderBody hitBody, Collision collision)
        {
            _thisEntityColliderBody = thisBody;
            _hitEntityColliderBody = hitBody;
            _thisCollider = collision.GetContact(0).thisCollider;
            _hitCollider = collision.collider;
        }

        public BodyCollision(EntityColliderBody thisBody, EntityColliderBody hitBody, Collider hitCollider)
        {
            _thisEntityColliderBody = thisBody;
            _hitEntityColliderBody = hitBody;
            _thisCollider = null;
            _hitCollider = hitCollider;
        }

        public bool IsEnemyCollision()
        {
            return _thisEntityColliderBody.Entity.IsEnemy(_hitEntityColliderBody.Entity);
        }

        public bool IsFriendlyCollision()
        {
            return _thisEntityColliderBody.Entity.IsFriendly(_hitEntityColliderBody.Entity);
        }
    }
}