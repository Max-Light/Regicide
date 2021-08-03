
using Mirror;
using System.Collections.Generic;

namespace Regicide.Game.Player
{
    public class GamePlayerKingdom : NetworkBehaviour
    {
        protected static Dictionary<int, PlayerFaction> _factions = new Dictionary<int, PlayerFaction>();
        protected PlayerFaction _kingdomFaction = null;

        public virtual int KingdomID { get => gameObject.GetInstanceID(); }
        public IReadOnlyCollection<GamePlayerKingdom> AlliedKingdoms { get => _kingdomFaction.AlliedKingdoms; }

        public void SetPlayerFaction(PlayerFaction faction)
        {
            AssignKingdomFaction(faction);
            OnKingdomFactionChange(faction);
        }

        protected void AssignKingdomFaction(PlayerFaction faction)
        {
            _kingdomFaction?.RemoveKingdom(this);
            if (_kingdomFaction?.AlliedKingdoms.Count == 0)
            {
                _factions.Remove(_kingdomFaction.FactionID);
            }

            if (faction == null)
            {
                faction = new PlayerFaction(this);
            }

            _kingdomFaction = faction;
            _kingdomFaction.AddKingdom(this);
        }

        public bool IsKingdomInFaction(GamePlayerKingdom kingdom) => _kingdomFaction.Equals(kingdom._kingdomFaction);

        protected virtual void OnKingdomFactionChange(PlayerFaction faction) { }

        protected virtual void Awake()
        {
            InitializeKingdomFaction();
        }

        protected void InitializeKingdomFaction()
        {
            // RETREIVE LOBBY PLAYER FACTION DATA AND APPLY IT TO THIS PLAYER FACTION FIELD
            if (_kingdomFaction == null)
            {
                _kingdomFaction = new PlayerFaction(this);
            }
            if (!_factions.ContainsKey(_kingdomFaction.FactionID))
            {
                _factions.Add(_kingdomFaction.FactionID, _kingdomFaction);
            }
        }
    }
}