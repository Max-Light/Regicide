
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Entities
{
    public class County : Entity
    {
        [SerializeField] protected List<Settlement> settlements = new List<Settlement>();
        public List<Settlement> Settlements { get => settlements; }

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