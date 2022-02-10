
using Regicide.Game.Entity.BodyCollision;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class TroopBattleFace : MonoBehaviour, SubcolliderPriorityAttributer
    {
        [SerializeField] private string _faceName = "";
        [SerializeField][Range(0, 100)] private int _battleLineLength = 0;
        [SerializeField][Range(0, 2)] private float _battleLineSplitLengthMultiplier = 1; 
        [SerializeField] private uint _colliderPriority = 0;
        private Dictionary<TroopBattleLine, int> _troopBattleLines = new Dictionary<TroopBattleLine, int>();

        public string FaceName { get => _faceName; }
        public int BattleLineLength { get => _battleLineLength; }
        public uint CollisionPriority { get => _colliderPriority; }
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
            int totalBattleLineLength = _battleLineLength;
            if (_troopBattleLines.Count == 1)
            {
                partitionAmount *= _battleLineSplitLengthMultiplier;
                totalBattleLineLength = (int)(totalBattleLineLength * _battleLineSplitLengthMultiplier);
            }
            float accumulatedPartition = 0;
            foreach (TroopBattleLine battleLine in _troopBattleLines.Keys.ToList())
            {
                int newBattleLineLength = (int)(accumulatedPartition + partitionAmount) - (int)accumulatedPartition;
                _troopBattleLines[battleLine] = newBattleLineLength;
                accumulatedPartition += partitionAmount;
            }
            List<TroopBattleUnit> shiftedBattleUnits = ShiftTroopBattleLines();
            TroopBattleLine troopBattleLine = new TroopBattleLine(this);
            _troopBattleLines.Add(troopBattleLine, totalBattleLineLength - (int)accumulatedPartition);
            troopBattleLine.AddRange(shiftedBattleUnits);
            return troopBattleLine;
        }

        public void DestroyTroopBattleLine(TroopBattleLine troopBattleLine)
        {
            if (_troopBattleLines.Remove(troopBattleLine) && _troopBattleLines.Count != 0)
            {
                troopBattleLine.Clear();

                float partitionAmount = _battleLineLength / _troopBattleLines.Count;
                float accumulatedPartition = 0;
                if (_troopBattleLines.Count > 1)
                {
                    partitionAmount *= _battleLineSplitLengthMultiplier;
                }

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