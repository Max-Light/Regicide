
using System.Collections;

namespace Regicide.Game.GameStates
{
    public abstract class GameServerState
    {
        public uint ClientStateId { get; protected set; } = 0;

        public GameServerState()
        {
            ClientStateId = GameClientState.Nil.GameStateId;
        }

        public class NilState : GameServerState { }
        public static NilState Nil { get; private set; } = new NilState();

        public virtual void OnStateEnable(ServerGameStateCycler cycler) { }
        public virtual IEnumerator OnUpdate(ServerGameStateCycler cycler) { return null; }
        public virtual void OnStateDisable(ServerGameStateCycler cycler) { }
    }
}