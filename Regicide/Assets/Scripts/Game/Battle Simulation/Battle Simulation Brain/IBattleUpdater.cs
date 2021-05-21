
namespace Regicide.Game.BattleSimulation
{
    public interface IBattleUpdater 
    {
        void StartBattle(BattleScenario battleScenario);
        void StopBattle(BattleScenario battleScenario);
    }
}