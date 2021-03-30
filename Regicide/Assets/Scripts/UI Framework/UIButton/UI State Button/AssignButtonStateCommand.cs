
using System;
using System.Collections.Generic;

namespace UI
{
    public class AssignButtonStateCommand<TState> : ICommand where TState : UIButtonState
    {
        private IEnumerable<UIStateButtonFramework> _stateButtons = null;

        public AssignButtonStateCommand(IEnumerable<UIStateButtonFramework> stateButtons)
        {
            this._stateButtons = stateButtons;
        }

        public void Execute()
        {
            foreach (UIStateButtonFramework button in _stateButtons)
            {
                TState state = Activator.CreateInstance(typeof(TState)) as TState;
                button.SetButtonState(state);
            }
        }
    }
}