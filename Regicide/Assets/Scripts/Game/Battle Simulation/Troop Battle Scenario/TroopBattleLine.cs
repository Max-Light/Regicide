
using Regicide.Game.Units;
using System.Collections.Generic;

namespace Regicide.Game.BattleSimulation
{
    public class TroopBattleLine : BattleLine<TroopBattleUnit>, IBattleLineDamager<TroopUnitDamage>, IBattleLineDamageable<TroopUnitDamage>
    {
        private TroopBattleFace _troopBattleFace = null;

        public int BattleLineLength { get => _troopBattleFace.GetBattleLineLength(this); }

        public IReadOnlyList<IBattleDamager<TroopUnitDamage>> BattleLineDamager => UnitBattleLine;
        public IReadOnlyList<IBattleDamageable<TroopUnitDamage>> BattleLineDamageable => UnitBattleLine;

        public TroopBattleLine(TroopBattleFace troopBattleFace)
        {
            _troopBattleFace = troopBattleFace;
        }

        public bool HasBelongingTroopBattleFace() { return _troopBattleFace != null; }

        public void FillBattleLine(TroopBattleRoster troopBattleRoster)
        {
            for (int battleIndex = UnitBattleLine.Count; battleIndex < BattleLineLength; battleIndex++)
            {
                if (troopBattleRoster.TrySelectBattleActiveTroopUnit(out TroopUnit troopUnit))
                {
                    TroopBattleUnit troopBattleUnit = new TroopBattleUnit(troopUnit);
                    Add(troopBattleUnit);
                }
                else
                {
                    break;
                }
            }
        }

        public int IndexOfTroopUnit(TroopUnit troopUnit)
        {
            for (int troopIndex = 0; troopIndex < UnitBattleLine.Count; troopIndex++)
            {
                if ((UnitBattleLine[troopIndex]).TroopUnit == troopUnit)
                {
                    return troopIndex;
                }
            }
            return -1;
        }

        public void Destroy()
        {
            _troopBattleFace.DestroyTroopBattleLine(this);
        }
    }
}