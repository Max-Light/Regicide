
namespace UI
{
    public abstract class UIStateButtonFramework : UIButtonFramework 
    {
        private UIButtonState state = null;

        protected virtual void Awake()
        {
            state = new NilButtonState(this);
        }

        public void SetButtonState(UIButtonState state)
        {
            if (state == null)
            {
                state = new NilButtonState(this);
            }
            this.state.OnStateDisable();
            this.state = state;
            this.state.OnStateEnable();
        }
    }
}