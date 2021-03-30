using Regicide.Game.Player;

namespace Regicide.Game.Entities
{
    public class AssignEntityCommand : EntityCommand
    {
        private GamePlayer _player = null;

        public AssignEntityCommand(Entity entity, GamePlayer player) : base(entity)
        {
            _player = player;
        }

        public override void Execute()
        {
            entity.AssignEntityOwnership(_player);
        }
    }
}