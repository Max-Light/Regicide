
namespace Regicide.Game.Units
{
    public class MilitiaArcherUnit : TroopUnit
    {
        public static Model TroopModel { get; private set; } = new Model(
            2,
            "Militia Archer",
            "",
            null
            );

        public override Model UnitModel => TroopModel;
    }
}