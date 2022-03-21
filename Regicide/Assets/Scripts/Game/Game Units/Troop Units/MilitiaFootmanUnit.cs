
namespace Regicide.Game.Units
{
    public class MilitiaFootmanUnit : TroopUnit
    {
        public static Model TroopModel { get; private set; } = new Model(
            1,
            "Militia Footman",
            "A basic militia unit",
            null
            );

        public override Model UnitModel => TroopModel;

        public MilitiaFootmanUnit()
        {
            SetPrimaryWeapon(new ShortSwordWeapon());
        }
    }
}