using Regicide.Game.Player;

namespace Regicide.Game.Entities 
{ 
    public interface IOwnableEntity
    {
        GamePlayer PlayerOwner { get; } 
        void AssignEntityToOwner(GamePlayer player);
        void RemoveEntityOwnership();
        bool HasOwner();
    }
}