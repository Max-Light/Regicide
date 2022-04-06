using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class BattleLineFormationAnchor : BattleLineFormationPoint
    {
        protected bool _active = false;

        public bool Active { get => _active; set => _active = value; }
    }
}