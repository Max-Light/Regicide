
namespace Regicide.Game.Units
{
    public class PeasantUnit : TroopUnit
    {
        public Model TroopModel { get; private set; } = new Model(
            3,
            "Peasant",
            "",
            null
            );

        public override Model UnitModel => TroopModel;
    }
}