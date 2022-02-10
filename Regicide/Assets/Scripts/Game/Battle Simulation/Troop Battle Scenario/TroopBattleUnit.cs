using Regicide.Game.Units;
using System;
using System.Collections;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class TroopBattleUnit : IBattleDamager<TroopUnitDamage>, IBattleDamageable<TroopUnitDamage>, IObservable<TroopBattleUnit>
    {
        private TroopUnit _troopUnit = null;
        private Action<TroopBattleUnit> _troopBattleUnitAction = null;
        private bool _canParry = true;

        public TroopUnit TroopUnit { get => _troopUnit; }

        public TroopBattleUnit(TroopUnit troopUnit)
        {
            SetTroopUnit(troopUnit);
        }

        public void SetTroopUnit(TroopUnit troopUnit)
        {
            _troopUnit = troopUnit;
        }

        public IEnumerator CommenceBattleFighting(IBattleDamageable<TroopUnitDamage> damageable)
        {
            if (_troopUnit != null)
            {
                TroopUnitDamage troopDamageInfliction = new TroopUnitDamage();
                float recoverTime = 1;
                float strikeDelay = 0;
                if (_troopUnit.TryGetWeaponOfType(out ITroopMeleeWeapon weapon))
                {
                    recoverTime = weapon.GetRecoverTime();
                    strikeDelay = weapon.GetStrikeDelayTime();
                    troopDamageInfliction = new TroopUnitDamage(weapon.DamageAttribute.DamageAmount);
                }
                yield return new WaitForSeconds(strikeDelay);
                damageable.ReceiveDamage(troopDamageInfliction);
                _canParry = false;
                yield return new WaitForSeconds(recoverTime);
                _canParry = true;
            }
            else
            {
                Debug.LogError("Troop Battle Unit has no occupying unit");
            }
        }

        public bool ReceiveDamage(TroopUnitDamage damageReport)
        {
            if (_canParry && _troopUnit.TryGetWeaponOfType(out ITroopMeleeWeapon weapon))
            {
                if (weapon.ParryChance <= UnityEngine.Random.Range(0, 1f))
                {
                    return false;
                }
            }
            _troopUnit.ReceiveDamage(damageReport.Damage);
            _troopBattleUnitAction?.Invoke(this);
            return true;
        }

        public void AddObserver(Action<TroopBattleUnit> action)
        {
            _troopBattleUnitAction += action;
        }

        public void RemoveObserver(Action<TroopBattleUnit> action)
        {
            _troopBattleUnitAction -= action;
        }
    }
}