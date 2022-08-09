
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public class AnchorArmControlPoint : MonoBehaviour, IControlPoint
    {
        private SplineAnchor _anchor = null;

        public SplineAnchor Anchor { get => _anchor; set => _anchor = value; }

        public void OnDeselect(BattleLineController controller)
        {
            
        }

        public void OnDragPoint(BattleLineController controller, Vector3 position)
        {
            
        }

        public void OnSelect(BattleLineController controller)
        {
            
        }
    }
}