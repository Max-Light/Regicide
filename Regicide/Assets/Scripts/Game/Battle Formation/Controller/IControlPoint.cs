using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public interface IControlPoint 
    {
        public Transform transform { get; }
        public void OnSelect(BattleLineController controller);
        public void OnDragPoint(BattleLineController controller, Vector3 position);
        public void OnDeselect(BattleLineController controller);
    }
}