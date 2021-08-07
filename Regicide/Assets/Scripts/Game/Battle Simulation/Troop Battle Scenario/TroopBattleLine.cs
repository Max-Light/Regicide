
using Regicide.Game.Units;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class TroopBattleLine : BattleLine<TroopBattleUnit>
    {
        private int _battleLineLength = 0;
        private TroopBattleFace _troopBattleFace = null;

        public int BattleLineLength 
        { 
            get => _battleLineLength;
            set => _battleLineLength = Mathf.Clamp(value, 0, int.MaxValue);
        }

        public TroopBattleFace TroopBattleFace { get => _troopBattleFace; }

        public TroopBattleLine(int battleLineLength, TroopBattleFace troopBattleFace)
        {
            _battleLine = new List<TroopBattleUnit>(battleLineLength);
            _battleLineLength = battleLineLength;
            _troopBattleFace = troopBattleFace;
        }

        public int IndexOfTroopUnit(TroopUnit troopUnit)
        {
            for (int troopIndex = 0; troopIndex < _battleLine.Count; troopIndex++)
            {
                if (_battleLine[troopIndex].TroopUnit == troopUnit)
                {
                    return troopIndex;
                }
            }
            return -1;
        }
    }
}