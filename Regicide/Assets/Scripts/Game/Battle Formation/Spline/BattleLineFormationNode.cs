using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class BattleLineFormationNode : BattleLineFormationPoint
    {
        [SerializeField] private BattleLineFormationAnchor _inAnchor;
        [SerializeField] private BattleLineFormationAnchor _outAnchor;
        private float _radius;
        private Vector3 _positionSnapshot;

        public BattleLineFormationAnchor InAnchor { get => _inAnchor; set => _inAnchor = value; }
        public BattleLineFormationAnchor OutAnchor { get => _outAnchor; set => _outAnchor = value; }
        public float Radius { get => _radius; set => _radius = value; }
        public Vector3 PositionSnapshot { get => _positionSnapshot; }

        public BattleLineFormationNode() 
        { 
            _radius = 1; 
        }

        public BattleLineFormationNode(int radius)
        {
            _radius = Radius;
        }

        public void SnapshotPosition()
        {
            _positionSnapshot = _position;
        }

        public bool IsSamePosition()
        {
            return _position == _positionSnapshot;
        }
    }
}