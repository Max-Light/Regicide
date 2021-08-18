
namespace Regicide.Game.EntityCollision
{
    public interface IEntityCollisionObserver
    {
        void OnEntityCollisionEnter(EntityColliderBrain thisBattleEntity, EntityColliderBrain hitBattleEntity, EntityCollision battleCollision);
        void OnEntityCollisionExit(EntityColliderBrain thisBattleEntity, EntityColliderBrain hitBattleEntity);
    }
}