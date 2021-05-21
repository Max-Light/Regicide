using Regicide.Game.Player;

namespace Regicide.Game.Entities 
{ 
    public interface IOwnableEntity
    {
        GamePlayer PlayerOwner { get; } 
        void AssignEntityOwnership(GamePlayer player);
        void RemoveEntityOwnership();
        bool HasOwner();
        bool IsFriendly(IOwnableEntity entity);
    }
}