using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public class NetworkBattleEntity : BattleEntity
    {
        protected override void OnCollisionEnter(Collision collision)
        {
            if (isServer)
            {
                base.OnCollisionEnter(collision);
            }
        }

        protected override void OnCollisionExit(Collision collision)
        {
            if (isServer)
            {
                base.OnCollisionExit(collision);
            }
        }
    }
}