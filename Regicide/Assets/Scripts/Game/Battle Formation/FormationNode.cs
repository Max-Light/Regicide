

using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class FormationNode : FormationPoint
    {
        protected FormationNode _prevNode = null;
        protected FormationNode _nextNode = null;
        protected FormationUnit _formationUnit = null;

        public FormationNode PrevNode { get => _prevNode; }
        public FormationNode NextNode { get => _nextNode; }
        public FormationUnit FormationUnit { get => _formationUnit; }
        public float Radius { get => _formationUnit.Radius; }

        public FormationNode(FormationNode prevNode, FormationUnit unit, Vector3 position) : base(position)
        {
            LinkNode(prevNode);
            _formationUnit = unit;
        }

        public FormationNode(FormationNode prevNode, Vector3 position) : base(position) 
        {
            LinkNode(prevNode);
        }

        public FormationNode(Vector3 position) : base(position) { }

        public void SetFormationUnit(FormationUnit formationUnit)
        {
            _formationUnit = formationUnit;
            MoveInFormation();
        }

        public void MoveInFormation()
        {
            Vector3 unitPosition = _formationUnit.transform.TransformPoint(_position);
            _formationUnit.NavAgent.CreatePath(unitPosition);
        }

        public void DetachNode()
        {
            _prevNode._nextNode = _nextNode;
            _nextNode._prevNode = _prevNode;
            _prevNode = null;
            _nextNode = null;
        }

        public void LinkNode(FormationNode prevNode)
        {
            if (prevNode != null)
            {
                _prevNode = prevNode;
                if (_prevNode._nextNode != null)
                {
                    _nextNode = prevNode._nextNode;
                    _nextNode._prevNode = this;
                }
                _prevNode._nextNode = this;
            }
        }
    }
}