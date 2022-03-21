
using System.Collections;

namespace Regicide.Game.GameStates
{
    public class GameClientState 
    {
        public uint GameStateId { get; protected set; } = 0;

        public class NilState : GameClientState { }
        public static NilState Nil { get; private set; } = new NilState();

        public virtual void OnStateEnable(ClientGameStateCycler cycler) { }
        public virtual void OnStateDisable(ClientGameStateCycler cycler) { }
    }
}