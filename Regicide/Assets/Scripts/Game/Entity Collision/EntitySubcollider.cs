
using UnityEngine;

namespace Regicide.Game.Entity
{
    [RequireComponent(typeof(Collider))]
    public class EntitySubcollider : MonoBehaviour
    {
        [Header("Collider Info")]
        [SerializeField] private uint _colliderPriority = 0;

        public uint CollisionPriority { get => _colliderPriority; }
    }
}