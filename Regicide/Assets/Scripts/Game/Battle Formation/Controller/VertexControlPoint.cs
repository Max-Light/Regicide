
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public class VertexControlPoint : MonoBehaviour, IControlPoint
    {
        private SplineNode _splineNode = null;

        public SplineNode SplineNode { get => _splineNode; set => _splineNode = value; }

        public void OnSelect(BattleLineController controller)
        {
            
        }

        public void OnDragPoint(BattleLineController controller, Vector3 position)
        {
            _splineNode.Position = position + BattleLineController.ActivatedControlPoint.pointerOffset;
        }

        public void OnDeselect(BattleLineController controller)
        {

        }

        private void Start()
        {
            transform.localPosition = _splineNode.Position;
        }

        private void Update()
        {
            transform.localPosition = _splineNode.Position;
        }
    }
}