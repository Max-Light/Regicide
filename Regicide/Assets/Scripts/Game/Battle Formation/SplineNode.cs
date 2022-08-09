
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class SplineNode : FormationNode
    {
        private Vector3 _previousPosition;

        public Vector3 PreviousPosition { get => _previousPosition; }

        public SplineNode(FormationNode prevNode, FormationUnit unit, Vector3 position) : base(prevNode, unit, position) 
        {
            _previousPosition = position;
        }

        public SplineNode(FormationUnit unit, Vector3 position) : base(null, unit, position) 
        {
            _previousPosition = position;
        }

        public SplineNode(FormationNode prevNode, Vector3 position) : base(prevNode, position) 
        {
            _previousPosition = position;
        }

        public SplineNode(Vector3 position) : base(null, position) 
        {
            _previousPosition = position;
        }

        public void SnapshotPosition()
        {
            _previousPosition = _position;
        }
    }
}