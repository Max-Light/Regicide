
namespace Regicide.Game.BattleSimulation
{
    public abstract class BattleScenario
    {
        public virtual void StartBattle() { }
        public virtual void StopBattle() { }
        public virtual void DestroyBattle() { }
    }
}