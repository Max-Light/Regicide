
using Regicide.UI;
using UI;
using UnityEngine;

namespace Regicide.Game.GameStates
{
    public class SetupClientState : GameClientState
    {
        public SetupClientState()
        {
            GameStateId = 2;
        }

        public override void OnStateEnable(ClientGameStateCycler cycler)
        {
            base.OnStateEnable(cycler);
            Debug.Log("Client in Setup State");

            ICommand countyButtonAssignment = new AssignButtonStateCommand<CountyAssignmentButtonState>(CountyUIButton.CountyButtons);
            countyButtonAssignment.Execute();
        }

        public override void OnStateDisable(ClientGameStateCycler cycler)
        {
            base.OnStateDisable(cycler);
        }
    }
}