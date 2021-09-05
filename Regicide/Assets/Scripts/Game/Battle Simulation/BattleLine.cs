
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public abstract class BattleLine<T> : IBattleLineObserver, IReadOnlyList<T> where T : IBattleUnit
    {
        protected List<T> _battleLine = new List<T>();
        protected Action<BattleLineOperation, int, IBattleUnit> _battleLineCallback = null;

        public IReadOnlyList<T> UnitBattleLine { get => _battleLine; }
        public int Count => _battleLine.Count;

        public T this[int index] { get => _battleLine [index]; }

        public static T1[][] PartitionBattleLine<T1>(IReadOnlyList<T1> battleLineUnits, int partitions, uint limit = 1) where T1 : IBattleUnit
        {
            T1[][] partitionedBattleLine = new T1[partitions][];
            if (partitions > 0 && battleLineUnits.Count > 0 && limit <= battleLineUnits.Count)
            {
                float partitionRatio = battleLineUnits.Count / partitions;
                float accumulatedAmount = 0;
                for (int partitionIndex = 0; partitionIndex < partitions; partitionIndex++)
                {
                    int startIndex = (int)accumulatedAmount;
                    accumulatedAmount += partitionRatio;
                    int battleUnitRange = Mathf.Clamp((int)accumulatedAmount - startIndex, (int)limit, battleLineUnits.Count);
                    T1[] battleUnits = new T1[battleUnitRange];
                    for (int battleUnitIndex = 0; battleUnitIndex < battleUnitRange; battleUnitIndex++)
                    {
                        battleUnits[battleUnitIndex] = battleLineUnits[startIndex + battleUnitIndex];
                    }
                    partitionedBattleLine[partitionIndex] = battleUnits;
                }
            }
            return partitionedBattleLine;
        }

        public void AddCallback(Action<BattleLineOperation, int, IBattleUnit> callback)
        {
            _battleLineCallback += callback;
        }

        public void RemoveCallback(Action<BattleLineOperation, int, IBattleUnit> callback)
        {
            _battleLineCallback -= callback;
        }

        public void Add(T battleUnit)
        {
            _battleLine.Add(battleUnit);
            _battleLineCallback?.Invoke(BattleLineOperation.OP_ADD, _battleLine.Count - 1, battleUnit);
        }

        public void Insert(int index, T battleUnit)
        {
            _battleLine.Insert(index, battleUnit);
            _battleLineCallback?.Invoke(BattleLineOperation.OP_INSERT, index, battleUnit);
        }

        public void AddRange(IEnumerable<T> battleUnits)
        {
            _battleLine.AddRange(battleUnits);
            foreach (IBattleUnit battleUnit in battleUnits)
            {
                _battleLineCallback?.Invoke(BattleLineOperation.OP_ADD, _battleLine.Count - 1, battleUnit);
            }
        }

        public bool Remove(T battleUnit)
        {
            int indexOfBattleUnit = _battleLine.IndexOf(battleUnit);
            if (indexOfBattleUnit != -1)
            {
                RemoveAt(indexOfBattleUnit);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            IBattleUnit battleUnit = _battleLine[index];
            _battleLine.RemoveAt(index);
            _battleLineCallback?.Invoke(BattleLineOperation.OP_REMOVEAT, index, battleUnit);
        }

        public void RemoveRange(int startIndex, int count)
        {
            List<T> battleUnits = _battleLine.GetRange(startIndex, count);
            _battleLine.RemoveRange(startIndex, count);
            for (int battleUnitIndex = 0; battleUnitIndex < battleUnits.Count; battleUnitIndex++)
            {
                _battleLineCallback?.Invoke(BattleLineOperation.OP_REMOVEAT, startIndex + battleUnitIndex, battleUnits[battleUnitIndex]);
            }
        }

        public void Clear()
        {
            RemoveRange(0, _battleLine.Count);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _battleLine.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _battleLine.GetEnumerator();
        }
    }
}