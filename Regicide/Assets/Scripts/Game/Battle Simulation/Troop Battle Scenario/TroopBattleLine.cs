
using Regicide.Game.Units;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class TroopBattleLine : BattleLine<TroopBattleUnit>, IBattleLineDamager<TroopUnitDamage>, IBattleLineDamageable<TroopUnitDamage>
    {
        private int _battleLineLength = 0;
        private TroopBattleFace _troopBattleFace = null;

        public int BattleLineLength 
        { 
            get => _battleLineLength;
            set => _battleLineLength = Mathf.Clamp(value, 0, int.MaxValue);
        }

        public TroopBattleFace TroopBattleFace { get => _troopBattleFace; }
        public IReadOnlyList<IBattleDamager<TroopUnitDamage>> BattleLineDamager => _battleLine;
        public IReadOnlyList<IBattleDamageable<TroopUnitDamage>> BattleLineDamageable => _battleLine;

        public TroopBattleLine(int battleLineLength, TroopBattleFace troopBattleFace)
        {
            _battleLine.Capacity = battleLineLength;
            _battleLineLength = battleLineLength;
            _troopBattleFace = troopBattleFace;
        }

        public int IndexOfTroopUnit(TroopUnit troopUnit)
        {
            for (int troopIndex = 0; troopIndex < _battleLine.Count; troopIndex++)
            {
                if (((TroopBattleUnit)_battleLine[troopIndex]).TroopUnit == troopUnit)
                {
                    return troopIndex;
                }
            }
            return -1;
        }
    }
}