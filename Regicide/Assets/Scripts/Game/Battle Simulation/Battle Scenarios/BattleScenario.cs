
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public abstract class BattleScenario
    {
        protected IBattleLine _battleLine_1 = null;
        protected IBattleLine _battleLine_2 = null;
        protected bool _isBattleActive = true;
        protected float _battleCycleTimeLength = 1.0f;

        public bool IsBattling { get => _isBattleActive; }
        public float BattleCycleTimeLength { get => _battleCycleTimeLength; }

        public BattleScenario(IBattleLine battleLine_1, IBattleLine battleLine_2)
        {
            _battleLine_1 = battleLine_1;
            _battleLine_2 = battleLine_2;
        }

        public void SetBattleCyclerTimeLength(float cycleTime)
        {
            _battleCycleTimeLength = Mathf.Clamp(cycleTime, 0.25f, 5);
        }

        public virtual void UpdateBattleInflictions() { }
    }
}