
using UnityEngine;

namespace Regicide.Game.Units
{
    public class DamageReductionAttribute
    {
        private static DamageReductionAttribute _none = new DamageReductionAttribute(0);

        public static DamageReductionAttribute None { get => _none; }
        public float DamageReduction { get; private set; } = 0;

        public DamageReductionAttribute(float damageReduction)
        {
            DamageReduction = Mathf.Clamp01(damageReduction);
        }
    }
}