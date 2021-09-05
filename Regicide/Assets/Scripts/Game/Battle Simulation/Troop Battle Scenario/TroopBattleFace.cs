
using Regicide.Game.Entity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    [RequireComponent(typeof(EntitySubcollider))]
    public class TroopBattleFace : MonoBehaviour
    {
        [SerializeField] private string _faceName = "";
        [SerializeField] [Range(0, 100)] private int _battleLineLength = 0;
        private List<TroopBattleLine> _battleLines = new List<TroopBattleLine>();

        public string FaceName { get => _faceName; }
        public int BattleLineLength { get => _battleLineLength; }
        public IReadOnlyList<TroopBattleLine> TroopBattleLines { get => _battleLines; }

        public TroopBattleLine CreateTroopBattleLine()
        {
            float partitionAmount = _battleLineLength / (_battleLines.Count + 1);
            float accumulatedPartition = 0;
            List<TroopBattleUnit> troopBattleUnits = new List<TroopBattleUnit>();
            for (int battleLineIndex = 0; battleLineIndex < _battleLines.Count; battleLineIndex++)
            {
                int newBattleLineLength = (int)(accumulatedPartition + partitionAmount) - (int)accumulatedPartition;
                accumulatedPartition += partitionAmount;
                _battleLines[battleLineIndex].BattleLineLength = newBattleLineLength;
                troopBattleUnits.AddRange(GetExcessTroopBattleUnits(_battleLines[battleLineIndex]));
            }
            TroopBattleLine troopBattleLine = new TroopBattleLine(_battleLineLength - (int)accumulatedPartition, this);
            troopBattleLine.AddRange(troopBattleUnits);
            _battleLines.Add(troopBattleLine);
            return troopBattleLine;
        }

        public void DestroyTroopBattleLine(TroopBattleLine troopBattleLine)
        {
            _battleLines.Remove(troopBattleLine);
            if (_battleLines.Count > 0)
            {
                float partitionAmount = _battleLineLength / _battleLines.Count;
                float accumulatedPartition = 0;
                for (int battleLineIndex = 0; battleLineIndex < _battleLines.Count; battleLineIndex++)
                {
                    int newBattleLineLength = (int)(accumulatedPartition + partitionAmount) - (int)accumulatedPartition;
                    accumulatedPartition += partitionAmount;
                    _battleLines[battleLineIndex].BattleLineLength = newBattleLineLength;
                }
            }
        }

        public bool HasTroopBattleLine(TroopBattleLine battleLine) => _battleLines.Contains(battleLine);

        public List<TroopBattleUnit> GetAllTroopBattleUnits()
        {
            List<TroopBattleUnit> troopBattleUnits = new List<TroopBattleUnit>();
            for (int battleLineIndex = 0; battleLineIndex < _battleLines.Count; battleLineIndex++)
            {
                troopBattleUnits.AddRange((IEnumerable<TroopBattleUnit>)_battleLines[battleLineIndex].UnitBattleLine);
            }
            return troopBattleUnits;
        }

        private List<TroopBattleUnit> GetExcessTroopBattleUnits(TroopBattleLine troopBattleLine)
        {
            List<IBattleUnit> excessTroopBattleUnits = new List<IBattleUnit>();
            IReadOnlyList<IBattleUnit> troopBattleUnits = troopBattleLine.UnitBattleLine;
            if (troopBattleUnits.Count > troopBattleLine.BattleLineLength) 
            {
                for (int troopIndex = troopBattleLine.BattleLineLength; troopIndex < troopBattleUnits.Count; troopIndex++)
                {
                    excessTroopBattleUnits.Add(troopBattleUnits[troopIndex]);
                }
                troopBattleLine.RemoveRange(troopBattleLine.BattleLineLength, troopBattleUnits.Count - troopBattleLine.BattleLineLength);
            }
            return (List<TroopBattleUnit>)excessTroopBattleUnits.Cast<TroopBattleUnit>();
        }
    }
}