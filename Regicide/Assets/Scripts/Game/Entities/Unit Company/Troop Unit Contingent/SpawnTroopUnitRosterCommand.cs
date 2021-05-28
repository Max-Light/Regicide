using Regicide.Game.Units;

namespace Regicide.Game.Entities
{
    public class SpawnTroopUnitRosterCommand : ICommand
    {
        private TroopUnitRoster _troopUnitRoster = null;
        private uint _troopUnitId = 0;
        private int _troopAmount = 0;

        public SpawnTroopUnitRosterCommand(TroopUnitRoster troopUnitRoster, uint troopUnitId, int troopAmount)
        {
            _troopUnitRoster = troopUnitRoster;
            _troopUnitId = troopUnitId;
            _troopAmount = troopAmount;
        }

        public void Execute()
        {
            for (int troopIndex = 0; troopIndex < _troopAmount; troopIndex++)
            {
                TroopUnit troopUnit = UnitFactory.GetUnit(_troopUnitId) as TroopUnit;
                if (troopUnit != null)
                {
                    _troopUnitRoster.AddTroop(troopUnit);
                }
            }
        }
    }
}