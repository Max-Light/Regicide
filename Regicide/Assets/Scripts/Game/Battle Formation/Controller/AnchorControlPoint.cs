
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public class AnchorControlPoint : MonoBehaviour, IControlPoint
    {
        [SerializeField] private AnchorArmControlPoint _outAnchor = null;
        [SerializeField] private AnchorArmControlPoint _inAnchor = null;

        public AnchorArmControlPoint OutAnchorArm { get => _outAnchor; set => _outAnchor = value; }
        public AnchorArmControlPoint InAnchorArm { get => _inAnchor; set => _inAnchor = value; }

        public void OnDeselect(BattleLineController controller)
        {
            
        }

        public void OnDragPoint(BattleLineController controller, Vector3 position)
        {
            
        }

        public void OnSelect(BattleLineController controller)
        {
            
        }

        private void Update()
        {
            
        }
    }
}