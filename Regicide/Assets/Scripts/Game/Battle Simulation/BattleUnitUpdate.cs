
using System.Collections;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public abstract class BattleUnitUpdate<T> 
    {
        private MonoBehaviour _monoBehaviour = null;
        private Coroutine _battleCoroutine = null;
        protected IBattleDamager<T> _damager = null;
        protected IBattleDamageable<T>[] _damageables = null;

        public IBattleDamager<T> BattleDamager 
        { 
            get => _damager; 
            set 
            {
                if (value == null)
                {
                    Stop();
                }
                else
                {
                    _damager = value;
                    Start();
                }
            } 
        }
        public IBattleDamageable<T>[] BattleDamageables 
        { 
            get => _damageables;
            set 
            {
                if (value == null || value.Length == 0)
                {
                    Stop();
                }
                else
                {
                    _damageables = value;
                }
            }
        }
        public bool IsBattling { get => _battleCoroutine != null; }

        public BattleUnitUpdate(MonoBehaviour monoBehaviour, IBattleDamager<T> damager, params IBattleDamageable<T>[] damageables)
        {
            _monoBehaviour = monoBehaviour;
            _damager = damager;
            _damageables = damageables;
        }

        public void Start()
        {
            Stop();
            _battleCoroutine = _monoBehaviour.StartCoroutine(UpdateBattle());
        }

        public void Stop()
        {
            if (IsBattling)
            {
                _monoBehaviour.StopCoroutine(_battleCoroutine);
                _battleCoroutine = null;
            }
        }

        protected virtual IEnumerator UpdateBattle() { yield return null; }
    }
}