
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    [RequireComponent(typeof(Collider))]
    public class BattleCollider : MonoBehaviour
    {
        [Header("Collider Info")]
        [SerializeField] private string _colliderName = "";
        [SerializeField][Range(0, 100)] private int _unitLength = 1;
        [SerializeField] private uint _colliderPriority = 0;

        public string ColliderName { get => _colliderName; }
        public int UnitLength { get => _unitLength; }
        public uint CollisionPriority { get => _colliderPriority; }
    }
}