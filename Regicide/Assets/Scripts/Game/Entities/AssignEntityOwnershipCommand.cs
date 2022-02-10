using Regicide.Game.Player;

namespace Regicide.Game.Entity
{
    public class AssignEntityOwnershipCommand : ICommand
    {
        private IEntity _entity = null;
        private GamePlayer _player = null;

        public AssignEntityOwnershipCommand(IEntity entity, GamePlayer player)
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