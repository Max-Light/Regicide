
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public class EdgeControlPoint : MonoBehaviour, IControlPoint
    {
        private FormationNode _formationNode = null;

        public FormationNode FormationNode { get => _formationNode; set => _formationNode = value; }

        public void OnDeselect(BattleLineController controller)
        {
            
        }

        public void OnDragPoint(BattleLineController controller, Vector3 position)
        {
            if (position != _formationNode.Position + BattleLineController.ActivatedControlPoint.pointerOffset)
            {
                controller.SetAsVertexPoint(this);
            }
        }

        public void OnSelect(BattleLineController controller)
        {
            
        }

        private void Start()
        {
            transform.localPosition = _formationNode.Position;
        }

        private void Update()
        {
            transform.localPosition = _formationNode.Position;
        }
    }
}