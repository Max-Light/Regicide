
namespace UI
{
    public abstract class UIStateButtonFramework : UIButtonFramework 
    {
        private UIButtonState _state = null;

        protected virtual void Awake()
        {
            _state = new NilButtonState(this);
        }

        public void SetButtonState(UIButtonState state)
        {
            if (state == null)
            {
                state = new NilButtonState(this);
            }
            this._state.OnStateDisable();
            this._state = state;
            this._state.OnStateEnable();
        }
    }
}