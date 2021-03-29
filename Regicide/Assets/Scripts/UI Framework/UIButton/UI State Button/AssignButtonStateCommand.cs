
using System;
using System.Collections.Generic;

namespace UI
{
    public class AssignButtonStateCommand<TState> : ICommand where TState : UIButtonState
    {
        private IEnumerable<UIStateButtonFramework> stateButtons = null;

        public AssignButtonStateCommand(IEnumerable<UIStateButtonFramework> stateButtons)
        {
            this.stateButtons = stateButtons;
        }

        public void Execute()
        {
            foreach (UIStateButtonFramework button in stateButtons)
            {
                TState state = Activator.CreateInstance(typeof(TState)) as TState;
                button.SetButtonState(state);
            }
        }
    }
}