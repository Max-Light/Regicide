using System.Collections;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class TroopUnitDamageBattleUpdate
    {
        private MonoBehaviour _monoBehaviour = null;
        private IEnumerator _battleCoroutine = null;
        private IBattleDamageable<TroopUnitDamage>[] _damageables = null;

        public TroopUnitDamageBattleUpdate(MonoBehaviour monoBehaviour, params IBattleDamageable<TroopUnitDamage>[] damageables)
        {
            _monoBehaviour = monoBehaviour;
            _damageables = damageables;
        }

        public void StartBattleUpdate(IBattleDamager<TroopUnitDamage> damager)
        {
            StopBattleUpdate();
            _battleCoroutine = BattleUpdate(damager);
            _monoBehaviour.StartCoroutine(_battleCoroutine);
        }

        public void StopBattleUpdate()
        {
            if (_battleCoroutine != null)
            {
                _monoBehaviour.StopCoroutine(_battleCoroutine);
                _battleCoroutine = null;
            }
        }

        public void SetDamageables(params IBattleDamageable<TroopUnitDamage>[] damageables)
        {
            _damageables = damageables;
        }

        public void ClearDamageables()
        {
            _damageables = null;
        }

        private IEnumerator BattleUpdate(IBattleDamager<TroopUnitDamage> damager)
        {
            while (_damageables != null && _damageables.Length != 0)
            {
                yield return damager.CommenceBattleFighting(_damageables[Random.Range(0, _damageables.Length)]);
            }
        }
    }
}