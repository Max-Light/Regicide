
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Entities
{
    public class Settlement : OwnableEntity
    {
        [SerializeField] protected County county = null;
        public County County { get => county; }

        public static Dictionary<uint, Settlement> Settlements { get; private set; } = new Dictionary<uint, Settlement>();

        public override void OnStartServer()
        {
            base.OnStartServer();
            Settlements.Add(netId, this);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isServer) { return; }
            Settlements.Add(netId, this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Settlements.Remove(netId);
        }
    }
}