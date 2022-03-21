
using Regicide.Game.Units;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public struct TroopUnitDamage 
    {
        private float _baseDamage;

        public float Damage { get => _baseDamage; }

        public TroopUnitDamage(float damage)
        {
            _baseDamage = Mathf.Clamp(damage, 0, float.MaxValue);
        }
    }
}