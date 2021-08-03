
using System.Collections.Generic;

namespace Regicide.Game.Player
{
    public class PlayerFaction
    {
        private HashSet<GamePlayerKingdom> _alliedPlayerKingdoms = new HashSet<GamePlayerKingdom>();

        public int FactionID { get; private set; }

        public PlayerFaction(GamePlayerKingdom playerKingdom)
        {
            FactionID = playerKingdom.KingdomID;
            AddKingdom(playerKingdom);
        }

        public IReadOnlyCollection<GamePlayerKingdom> AlliedKingdoms => _alliedPlayerKingdoms;

        public void AddKingdom(GamePlayerKingdom kingdom)
        {
            _alliedPlayerKingdoms.Add(kingdom);
        }

        public void RemoveKingdom(GamePlayerKingdom kingdom)
        {
            _alliedPlayerKingdoms.Remove(kingdom);
        }
    }
}