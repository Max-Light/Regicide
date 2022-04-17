
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class BattleLineSplineNode : BattleLineNode
    {
        [SerializeField] private BattleLineSplineAnchor _inAnchor;
        [SerializeField] private BattleLineSplineAnchor _outAnchor;
        private Vector3 _previousPosition;

        public BattleLineSplineAnchor InAnchor { get => _inAnchor; set => _inAnchor = value; }
        public BattleLineSplineAnchor OutAnchor { get => _outAnchor; set => _outAnchor = value; }
        public Vector3 PreviousPosition { get => _previousPosition; }

        public bool IsSamePosition => _previousPosition == _position;

        public BattleLineSplineNode(BattleLineNode node) : base(node.Radius)
        {
            _position = node.Position;
            _previousPosition = node.Position;
        }

        public BattleLineSplineNode(float radius) : base(radius) { }
        public BattleLineSplineNode() { }

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