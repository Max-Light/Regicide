
using UnityEngine;

namespace Regicide.Game.EntityCollision
{
    [RequireComponent(typeof(Collider))]
    public class EntityCollider : MonoBehaviour
    {
        [Header("Collider Info")]
        [SerializeField] private uint _colliderPriority = 0;

        public uint CollisionPriority { get => _colliderPriority; }
    }
}