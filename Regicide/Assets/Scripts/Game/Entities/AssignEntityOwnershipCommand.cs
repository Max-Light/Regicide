using Regicide.Game.Player;

namespace Regicide.Game.Entities
{
    public class AssignEntityOwnershipCommand : ICommand
    {
        private IOwnableEntity _entity = null;
        private GamePlayer _player = null;

        public AssignEntityOwnershipCommand(IOwnableEntity entity, GamePlayer player)
        {
            _entity = entity;
            _player = player;
        }

        public void Execute()
        {
            _entity.AssignEntityOwnership(_player);
        }
    }
}