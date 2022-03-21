
namespace Regicide.Game.BattleSimulation
{
    public abstract class BattleScenario
    {
        private bool _isBattling = false;

        public bool IsBattling { get => _isBattling; }

        public virtual void StartBattle() { _isBattling = true; }
        public virtual void StopBattle() { _isBattling = false; }
        public virtual void EndBattle() { StopBattle(); }
    }
}