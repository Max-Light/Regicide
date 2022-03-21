
namespace UI
{
    public abstract class UIButtonState
    {
        public UIButtonState(UIStateButtonFramework stateButton) { }

        public virtual void OnStateEnable() { }
        public virtual void OnStateDisable() { }
    }

    public class NilButtonState : UIButtonState
    {
        public NilButtonState(UIStateButtonFramework stateButton) : base(stateButton) { }
    }
}