
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class BattleLineFormationNode : BattleLineFormationPoint
    {
        [SerializeField] private BattleLineFormationAnchor _inAnchor;
        [SerializeField] private BattleLineFormationAnchor _outAnchor;
        private float _radius;
        private Vector3 _previousPosition;

        public BattleLineFormationAnchor InAnchor { get => _inAnchor; set => _inAnchor = value; }
        public BattleLineFormationAnchor OutAnchor { get => _outAnchor; set => _outAnchor = value; }
        public float Radius { get => _radius; set => _radius = value; }
        public Vector3 PreviousPosition { get => _previousPosition; }

        public bool IsSamePosition => _previousPosition == _position;

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
            _previousPosition = _position;
        }

        public void OffsetPosition(Vector3 offset)
        {
            _position += offset;
            _previousPosition += offset;
        }
    }
}