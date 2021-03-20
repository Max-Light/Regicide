
namespace Regicide.Game.GameStates
{
    public class ServerGameState
    {
        public class NilState : ServerGameState { }
        public static NilState Nil { get; private set; } = new NilState();

        public virtual void OnStateEnable(ServerGameStateCycler cycler) { }
        public virtual void OnStateDisable(ServerGameStateCycler cycler) { }
    }
}