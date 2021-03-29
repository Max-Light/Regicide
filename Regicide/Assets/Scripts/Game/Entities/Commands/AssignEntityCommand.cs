using Regicide.Game.Player;

namespace Regicide.Game.Entities
{
    public class AssignEntityCommand : EntityCommand
    {
        private GamePlayer player = null;

        public AssignEntityCommand(Entity entity, GamePlayer player) : base(entity)
        {
            this.player = player;
        }

        public override void Execute()
        {
            entity.AssignEntityOwnership(player);
        }
    }
}