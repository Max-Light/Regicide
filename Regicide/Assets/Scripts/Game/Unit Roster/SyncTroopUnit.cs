
namespace Regicide.Game.Units
{
    public struct SyncTroopUnit
    {
        public uint unitId;
        public float health;

        public SyncTroopUnit(TroopUnit troopUnit)
        {
            unitId = troopUnit.UnitModel.UnitId;
            health = troopUnit.Health;
        }
    }
}