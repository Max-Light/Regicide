using UnityEngine;

namespace Regicide.Game.Entity
{
    public class NetworkEntityColliderBrain : EntityColliderBrain
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