
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
        private Dictionary<TroopBattleLine, int> _troopBattleLines = new Dictionary<TroopBattleLine, int>();

        public string FaceName { get => _faceName; }
        public int BattleLineLength { get => _battleLineLength; }
        public List<TroopBattleLine> TroopBattleLines { get => _troopBattleLines.Keys.ToList(); }

        public int GetBattleLineLength(TroopBattleLine troopBattleLine)
        {
            if (_troopBattleLines.TryGetValue(troopBattleLine, out int battleLineLength))
            {
                return battleLineLength;
            }
            return 0;
        }

        public TroopBattleLine CreateTroopBattleLine()
        {
            float partitionAmount = _battleLineLength / (_troopBattleLines.Count + 1);
            float accumulatedPartition = 0;
            foreach (TroopBattleLine battleLine in _troopBattleLines.Keys.ToList())
            {
                int newBattleLineLength = (int)(accumulatedPartition + partitionAmount) - (int)accumulatedPartition;
                _troopBattleLines[battleLine] = newBattleLineLength;
                accumulatedPartition += partitionAmount;
            }
            List<TroopBattleUnit> shiftedBattleUnits = ShiftTroopBattleLines();
            TroopBattleLine troopBattleLine = new TroopBattleLine(this);
            _troopBattleLines.Add(troopBattleLine, _battleLineLength - (int)accumulatedPartition);
            troopBattleLine.AddRange(shiftedBattleUnits);
            return troopBattleLine;
        }

        public void DestroyTroopBattleLine(TroopBattleLine troopBattleLine)
        {
            if (_troopBattleLines.ContainsKey(troopBattleLine) && _troopBattleLines.Remove(troopBattleLine) && _troopBattleLines.Count != 0)
            {
                troopBattleLine.Clear();

                float partitionAmount = _battleLineLength / _troopBattleLines.Count;
                float accumulatedPartition = 0;

                foreach (TroopBattleLine battleLine in _troopBattleLines.Keys.ToList())
                {
                    int newBattleLineLength = (int)(accumulatedPartition + partitionAmount) - (int)accumulatedPartition;
                    _troopBattleLines[battleLine] = newBattleLineLength;
                    accumulatedPartition += partitionAmount;
                }
            }
        }

        public List<TroopBattleUnit> ShiftTroopBattleLines()
        {
            List<TroopBattleUnit> excessTroopBattleUnits = new List<TroopBattleUnit>();
            foreach (TroopBattleLine battleLine in _troopBattleLines.Keys)
            {
                if (excessTroopBattleUnits.Count != 0)
                {
                    battleLine.InsertRange(0, excessTroopBattleUnits);
                }

                int battleLineLength = battleLine.BattleLineLength;
                if (battleLine.Count > battleLineLength)
                {
                    excessTroopBattleUnits = battleLine.RemoveRange(battleLineLength, battleLine.Count - battleLineLength);
                }
            }
            return excessTroopBattleUnits;
        }

        public bool HasTroopBattleLine(TroopBattleLine battleLine) => _troopBattleLines.ContainsKey(battleLine);

        public List<TroopBattleUnit> GetAllTroopBattleUnits()
        {
            List<TroopBattleUnit> troopBattleUnits = new List<TroopBattleUnit>();
            foreach (TroopBattleLine battleLine in _troopBattleLines.Keys)
            {
                troopBattleUnits.AddRange(battleLine);
            }
            return troopBattleUnits;
        }
    }
}