
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Entities
{
    public class County : OwnableEntity
    {
        [SerializeField] protected Settlement[] settlements = null;
        public Settlement[] Settlements { get => settlements; }

        public static Dictionary<uint, County> Counties { get; private set; } = new Dictionary<uint, County>();

        public override void OnStartServer()
        {
            base.OnStartServer();
            Counties.Add(netId, this);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isServer) { return; }
            Counties.Add(netId, this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Counties.Remove(netId);
        }
    }
}