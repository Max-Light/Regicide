using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class BattleLineFormationAnchor : BattleLineFormationPoint
    {
        private Vector3 _previousPosition;

        public Vector3 PreviousPosition { get => _previousPosition; }

        public bool IsSamePosition => _previousPosition == _position;

        public void SnapshotPosition()
        {
            _previousPosition = _position;
        }

        public void OffsetPosition(Vector3 offset)
        {
            _position += offset;
            _previousPosition += offset;
        }
    }
}