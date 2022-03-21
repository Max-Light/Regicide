
using UI;

namespace Regicide.UI
{
    public class CountyAssignmentButtonState : UIButtonState
    {
        private CountyUIButton _button = null;

        public CountyAssignmentButtonState(CountyUIButton button) : base(button)
        {
            this._button = button;
        }

        public override void OnStateEnable()
        {
            base.OnStateEnable();

        }

        public override void OnStateDisable()
        {
            base.OnStateDisable();

        }
    }
}