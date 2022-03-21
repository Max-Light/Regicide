using Mirror;

namespace Regicide.Game.Player
{
    public class NetworkGamePlayerKingdom : GamePlayerKingdom
    {
        [SyncVar(hook = nameof(OnFactionChange))]
        private int _factionId = 0;
        public override int KingdomID => (int)netId;

        [Server]
        protected override void OnKingdomFactionChange(PlayerFaction faction)
        {
            _factionId = faction.FactionID;
        }

        private void OnFactionChange(int _, int factionId)
        {
            AssignKingdomFaction(_factions[factionId]);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            InitializeKingdomFaction();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isServer) { return; }
            InitializeKingdomFaction();
        }

        protected override void Awake() { }
    }
}