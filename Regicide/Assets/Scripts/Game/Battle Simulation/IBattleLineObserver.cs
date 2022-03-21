using System;

namespace Regicide.Game.BattleSimulation
{
    public interface IBattleLineObserver
    {
        void AddBattleLineLengthCallback(Action callback);
        void RemoveBattleLineLengthCallback(Action callback);
    }
}