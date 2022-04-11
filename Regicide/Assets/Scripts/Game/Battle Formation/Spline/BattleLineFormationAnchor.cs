using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class BattleLineFormationAnchor : BattleLineFormationPoint
    {
        private Vector3 _positionSnapshot;
        private Quaternion _rotation = Quaternion.identity;
        private Quaternion _rotationSnapshot;

        public Vector3 PositionSnapshot { get => _positionSnapshot; }
        public Quaternion Rotation { get => _rotation; set => _rotation = value; }
        public Quaternion RotationSnapShot { get => _rotationSnapshot; }

        public void SnapshotPosition()
        {
            _positionSnapshot = _position;
        }

        public void SnapshotRotation()
        {
            _rotationSnapshot = _rotation;
        }

        public bool IsSamePosition()
        {
            return _position == _positionSnapshot;
        }

        public bool IsSameRotation()
        {
            return _rotation == _rotationSnapshot;
        }
    }
}