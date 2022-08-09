
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class SplineAnchor : FormationPoint
    {
        private Vector3 _previousPosition;

        public Vector3 PreviousPosition { get => _previousPosition; }

        public SplineAnchor(Vector3 position) : base(position)
        {
            _previousPosition = position;
        }

        public void SnapshotPosition()
        {
            _previousPosition = _position;
        }
    }
}