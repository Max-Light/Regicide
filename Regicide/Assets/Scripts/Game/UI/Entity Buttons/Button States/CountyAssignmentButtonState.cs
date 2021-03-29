
using UI;

namespace Regicide.UI
{
    public class CountyAssignmentButtonState : UIButtonState
    {
        private CountyUIButton button = null;

        public CountyAssignmentButtonState(CountyUIButton button) : base(button)
        {
            this.button = button;
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