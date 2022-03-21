using System.Collections;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class TroopUnitDamageBattleUpdate : BattleUnitUpdate<TroopUnitDamage>
    {
        public TroopUnitDamageBattleUpdate(MonoBehaviour monoBehaviour, IBattleDamager<TroopUnitDamage> damager, params IBattleDamageable<TroopUnitDamage>[] damageables) : base(monoBehaviour, damager, damageables) { }

        protected override IEnumerator UpdateBattle()
        {
            while (_damageables != null && _damageables.Length != 0)
            {
                yield return _damager.CommenceBattleFighting(_damageables[Random.Range(0, _damageables.Length)]);
            }
        }
    }
}