
namespace Regicide.Game.Entity
{
    public interface IEntityCollisionEnterObserver
    {
        void OnEntityCollisionEnter(EntityCollision battleCollision);
    }
}