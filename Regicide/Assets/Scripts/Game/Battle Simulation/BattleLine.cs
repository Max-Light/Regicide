
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public abstract class BattleLine<T> : IReadOnlyList<T>, IObservable
    {
        public enum Operation 
        {
            OP_ADD,
            OP_INSERT,
            OP_REMOVEAT,
        }

        protected List<T> _battleLine = new List<T>();
        protected Action<Operation, int, T> _battleLineCallback = null;
        protected Action _battleLineLengthCallback = null;

        public IReadOnlyList<T> UnitBattleLine { get => _battleLine; }
        public int Count => _battleLine.Count;

        public T this[int index] { get => _battleLine [index]; }

        public static T[][] PartitionBattleLine(IReadOnlyList<T> battleLineUnits, int partitions, uint limit = 1)
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

        public static bool ContainsBattleUnitOfType<T1>()
        {
            return typeof(T1).IsAssignableFrom(typeof(T));
        }

        public void AddObserver(Action action)
        {
            _battleLineLengthCallback += action;
        }

        public void RemoveObserver(Action action)
        {
            _battleLineLengthCallback -= action;
        }

        public void AddCallback(Action<Operation, int, T> callback)
        {
            _battleLineCallback += callback;
        }

        public void RemoveCallback(Action<Operation, int, T> callback)
        {
            _battleLineCallback -= callback;
        }

        public void Add(T battleUnit)
        {
            _battleLine.Add(battleUnit);
            _battleLineCallback?.Invoke(Operation.OP_ADD, _battleLine.Count - 1, battleUnit);
            _battleLineLengthCallback?.Invoke();
        }

        public void Insert(int index, T battleUnit)
        {
            _battleLine.Insert(index, battleUnit);
            _battleLineCallback?.Invoke(Operation.OP_INSERT, index, battleUnit);
            _battleLineLengthCallback?.Invoke();
        }

        public void AddRange(IEnumerable<T> battleUnits)
        {
            _battleLine.AddRange(battleUnits);
            foreach (T battleUnit in battleUnits)
            {
                _battleLineCallback?.Invoke(Operation.OP_ADD, _battleLine.Count - 1, battleUnit);
            }
            _battleLineLengthCallback?.Invoke();
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
            T battleUnit = _battleLine[index];
            _battleLine.RemoveAt(index);
            _battleLineCallback?.Invoke(Operation.OP_REMOVEAT, index, battleUnit);
            _battleLineLengthCallback?.Invoke();
        }

        public void RemoveRange(int startIndex, int count)
        {
            List<T> battleUnits = _battleLine.GetRange(startIndex, count);
            _battleLine.RemoveRange(startIndex, count);
            for (int battleUnitIndex = 0; battleUnitIndex < battleUnits.Count; battleUnitIndex++)
            {
                _battleLineCallback?.Invoke(Operation.OP_REMOVEAT, startIndex + battleUnitIndex, battleUnits[battleUnitIndex]);
            }
            _battleLineLengthCallback?.Invoke();
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