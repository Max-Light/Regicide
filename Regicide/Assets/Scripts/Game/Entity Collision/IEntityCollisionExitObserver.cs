
namespace Regicide.Game.Entity
{
    public interface IEntityCollisionExitObserver
    {
        void OnEntityCollisionExit(EntityColliderBrain thisBattleEntity, EntityColliderBrain hitBattleEntity);
    }
}