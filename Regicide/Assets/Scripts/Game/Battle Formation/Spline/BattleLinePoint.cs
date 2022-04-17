
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class BattleLinePoint 
    {
        [SerializeField] protected Vector3 _position;

        public Vector3 Position { get => _position; set => _position = value; }
    }
}