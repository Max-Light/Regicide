
namespace Regicide.Game.Entities
{
    public abstract class EntityCommand : ICommand
    {
        protected Entity entity = null;

        public EntityCommand(Entity entity)
        {
            this.entity = entity;
        }

        public virtual void Execute() { }
    }
}