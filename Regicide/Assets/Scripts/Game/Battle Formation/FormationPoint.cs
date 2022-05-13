
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class FormationPoint 
    {
        [SerializeField] protected Vector3 _position;
        protected Vector3 _tangent;

        public Vector3 Position { get => _position; set => _position = value; }
        public Vector3 Tangent { get => _tangent; set => _tangent = value; }

        public FormationPoint(Vector3 position)
        {
            _position = position;
        }
    }
}