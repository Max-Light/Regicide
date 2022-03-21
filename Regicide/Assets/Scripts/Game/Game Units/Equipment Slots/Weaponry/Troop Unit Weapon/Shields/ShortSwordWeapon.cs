
using UnityEngine;

namespace Regicide.Game.Units
{
    public class ShortSwordWeapon : TroopUnitWeapon, ITroopOneHandedMeleeWeapon
    {
        private static DamageAttribute _damage = new DamageAttribute(14.25f, 20.15f);
        private static float _minRecoverTime = 1.125f;
        private static float _maxRecoverTime = 1.150f;
        private static float _minStrikeDelay = 0.72f;
        private static float _maxStrikeDelay = 1.36f;
        private static float _parryChance = 0.45f;

        private ITroopShieldWeapon _shield = null;

        public DamageAttribute DamageAttribute => _damage;
        public float MinRecoverTime => _minRecoverTime;
        public float MaxRecoverTime => _maxRecoverTime;
        public float MinStrikeDelay => _minStrikeDelay;
        public float MaxStrikeDelay => _maxStrikeDelay;
        public float ParryChance => _parryChance;
        public ITroopShieldWeapon ShieldWeapon => _shield;

        public void EquipShield(ITroopShieldWeapon shield)
        {
            _shield = shield;
        }

        public void UnequipShield()
        {
            _shield = null;
        }

        public float GetRecoverTime()
        {
            float recoverTime = Random.Range(_minRecoverTime, _maxRecoverTime);
            if (HasShieldEquipped())
            {
                recoverTime += _shield.RecoverTimePenalty;
            }
            return recoverTime;
        }

        public float GetStrikeDelayTime()
        {
            float strikeDelay = Random.Range(_minStrikeDelay, _maxStrikeDelay);
            if (HasShieldEquipped())
            {
                strikeDelay += _shield.StrikeDelayPenalty;
            }
            return strikeDelay;
        }

        public bool HasShieldEquipped()
        {
            return _shield != null;
        }
    }
}