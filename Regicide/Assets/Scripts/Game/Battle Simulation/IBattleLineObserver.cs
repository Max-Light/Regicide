
using System;

namespace Regicide.Game.BattleSimulation
{
    public interface IBattleLineObserver 
    {
        void AddCallback(Action<BattleLineOperation, int, IBattleUnit> callback);
        void RemoveCallback(Action<BattleLineOperation, int, IBattleUnit> callback);
    }
}