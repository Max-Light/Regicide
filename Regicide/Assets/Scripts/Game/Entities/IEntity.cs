using Regicide.Game.Player;

namespace Regicide.Game.Entity 
{ 
    public interface IEntity
    {
        int EntityId { get; }
        GamePlayer PlayerOwner { get; } 
        void AssignEntityOwnership(GamePlayer player);
        void RemoveEntityOwnership();
        bool HasOwner();
        bool IsFriendly(IEntity entity);
        bool IsEnemy(IEntity entity);
    }
}