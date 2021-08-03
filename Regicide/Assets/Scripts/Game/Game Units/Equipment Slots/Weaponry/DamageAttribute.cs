

using UnityEngine;

namespace Regicide.Game.Units
{
    public class DamageAttribute 
    {
        private static DamageAttribute _none = new DamageAttribute(0);

        public static DamageAttribute None { get => _none; }
        public float MinDamageAmount { get; private set; } = 0;
        public float MaxDamageAmount { get; private set; } = 0;
        public float DamageAmount { get => Random.Range(MinDamageAmount, MaxDamageAmount); }

        public DamageAttribute(float minDamage, float maxDamage)
        {
            MinDamageAmount = minDamage;
            MaxDamageAmount = maxDamage;
        }

        public DamageAttribute(float damage)
        {
            MinDamageAmount = damage;
            MaxDamageAmount = damage;
        }
    }
}