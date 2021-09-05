
namespace Regicide.Game.Entity
{
    public struct EntityCollision 
    {
        private EntityColliderBrain _thisEntityColliderBrain;
        private EntityColliderBrain _hitEntityColliderBrain;
        private EntitySubcollider _thisSubcollider;
        private EntitySubcollider _hitSubcollider;

        public EntityColliderBrain ThisEntityColliderBrain { get => _thisEntityColliderBrain; }
        public EntityColliderBrain HitEntityColliderBrain { get => _hitEntityColliderBrain; }
        public EntitySubcollider ThisBattleCollider { get => _thisSubcollider; }
        public EntitySubcollider HitBattleCollider { get => _hitSubcollider; }

        public EntityCollision(EntityColliderBrain thisEntity, EntityColliderBrain hitEntity, EntitySubcollider thisSubcollider, EntitySubcollider hitSubcollider)
        {
            _thisEntityColliderBrain = thisEntity;
            _hitEntityColliderBrain = hitEntity;
            _thisSubcollider = thisSubcollider;
            _hitSubcollider = hitSubcollider;
        }

        public bool IsCollidedEntitiesEnemies()
        {
            return _thisEntityColliderBrain.Entity.IsEnemy(_hitEntityColliderBrain.Entity);
        }

        public bool IsCollidedEntitiesFriendly()
        {
            return _thisEntityColliderBrain.Entity.IsFriendly(_hitEntityColliderBrain.Entity);
        }
    }
}