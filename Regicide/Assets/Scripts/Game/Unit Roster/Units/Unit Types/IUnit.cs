
namespace Regicide.Game.Units
{
    public interface IUnit
    {
        UnitModel Model { get; }
        float Health { get; }

        void TakeDamage(float damage);
    }
}