
namespace Regicide.Game.Units
{
    public interface IUnit
    {
        Unit.Model UnitModel { get; }
        float Health { get; }
    }
}