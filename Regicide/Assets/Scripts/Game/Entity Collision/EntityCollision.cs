

namespace Regicide.Game.EntityCollision
{
    public class EntityCollision 
    {
        public EntityCollider ThisBattleCollider { get; private set; } = null;
        public EntityCollider HitBattleCollider { get; private set; } = null;

        public EntityCollision(EntityCollider thisBattleCollider, EntityCollider hitBattleCollider)
        {
            ThisBattleCollider = thisBattleCollider;
            HitBattleCollider = hitBattleCollider;
        }
    }
}