
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public abstract class BattleLine<T> : BattleLine, IBattleLineObserver, IReadOnlyList<T> where T : IBattleUnit
    {
        public enum CallbackOperation
        {
            OP_ADD,
            OP_INSERT,
            OP_REMOVEAT,
        }

        private List<T> _unitBattleLine = new List<T>();
        protected Action<CallbackOperation, int, T> _battleUnitCallback = null;
        protected Action _battleLineLengthCallback = null;

        public IReadOnlyList<T> UnitBattleLine { get => _unitBattleLine; }
        public int Count => _unitBattleLine.Count;

        public T this[int index] { get => _unitBattleLine [index]; }

        public void AddBattleUnitCallback(Action<CallbackOperation, int, T> callback)
        {
            _battleUnitCallback += callback;
        }

        public void RemoveBattleUnitCallback(Action<CallbackOperation, int, T> callback)
        {
            _battleUnitCallback -= callback;
        }

        public void AddBattleLineLengthCallback(Action callback)
        {
            _battleLineLengthCallback += callback;
        }

        public void RemoveBattleLineLengthCallback(Action callback)
        {
            _battleLineLengthCallback -= callback;
        }

        public void Add(T battleUnit)
        {
            _unitBattleLine.Add(battleUnit);
            _battleUnitCallback?.Invoke(CallbackOperation.OP_ADD, _unitBattleLine.Count - 1, battleUnit);
            _battleLineLengthCallback?.Invoke();
        }

        public void AddRange(IEnumerable<T> battleUnits)
        {
            _unitBattleLine.AddRange(battleUnits);
            foreach (T battleUnit in battleUnits)
            {
                _battleUnitCallback?.Invoke(CallbackOperation.OP_ADD, _unitBattleLine.Count - 1, battleUnit);
            }
            _battleLineLengthCallback?.Invoke();
        }

        public void Insert(int index, T battleUnit)
        {
            _unitBattleLine.Insert(index, battleUnit);
            _battleUnitCallback?.Invoke(CallbackOperation.OP_INSERT, index, battleUnit);
            _battleLineLengthCallback?.Invoke();
        }

        public void InsertRange(int index, IEnumerable<T> battleUnits)
        {
            _unitBattleLine.InsertRange(index, battleUnits);
            int indexOffset = 0;
            foreach (T battleUnit in battleUnits)
            {
                _battleUnitCallback?.Invoke(CallbackOperation.OP_INSERT, index + indexOffset, battleUnit);
                indexOffset++;
            }
            _battleLineLengthCallback?.Invoke();
        }

        public bool Remove(T battleUnit)
        {
            int indexOfBattleUnit = _unitBattleLine.IndexOf(battleUnit);
            if (indexOfBattleUnit != -1)
            {
                RemoveAt(indexOfBattleUnit);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            T battleUnit = _unitBattleLine[index];
            _unitBattleLine.RemoveAt(index);
            _battleUnitCallback?.Invoke(CallbackOperation.OP_REMOVEAT, index, battleUnit);
            _battleLineLengthCallback?.Invoke();
        }

        public List<T> RemoveRange(int startIndex, int count)
        {
            List<T> battleUnits = _unitBattleLine.GetRange(startIndex, count);
            _unitBattleLine.RemoveRange(startIndex, count);
            for (int battleUnitIndex = 0; battleUnitIndex < battleUnits.Count; battleUnitIndex++)
            {
                _battleUnitCallback?.Invoke(CallbackOperation.OP_REMOVEAT, startIndex + battleUnitIndex, battleUnits[battleUnitIndex]);
            }
            _battleLineLengthCallback?.Invoke();
            return battleUnits;
        }

        public List<T> GetRange(int startIndex, int count)
        {
            return _unitBattleLine.GetRange(startIndex, count);
        }

        public void Clear()
        {
            RemoveRange(0, _unitBattleLine.Count);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _unitBattleLine.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _unitBattleLine.GetEnumerator();
        }
    }

    public abstract class BattleLine
    {
        public static T[][] PartitionBattleLine<T>(IReadOnlyList<T> battleLineUnits, int partitions, uint limit = 1) where T : IBattleUnit
        {
            T[][] partitionedBattleLine = new T[partitions][];
            if (partitions > 0 && battleLineUnits.Count > 0 && limit <= battleLineUnits.Count)
            {
                float partitionRatio = battleLineUnits.Count / partitions;
                float accumulatedAmount = 0;
                for (int partitionIndex = 0; partitionIndex < partitions; partitionIndex++)
                {
                    int startIndex = (int)accumulatedAmount;
                    accumulatedAmount += partitionRatio;
                    int battleUnitRange = Mathf.Clamp((int)accumulatedAmount - startIndex, (int)limit, battleLineUnits.Count);
                    T[] battleUnits = new T[battleUnitRange];
                    for (int battleUnitIndex = 0; battleUnitIndex < battleUnitRange; battleUnitIndex++)
                    {
                        battleUnits[battleUnitIndex] = battleLineUnits[startIndex + battleUnitIndex];
                    }
                    partitionedBattleLine[partitionIndex] = battleUnits;
                }
            }
            return partitionedBattleLine;
        }
    }
}