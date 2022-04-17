
namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class BattleLineNode : BattleLinePoint
    {
        protected float _radius;

        public float Radius { get => _radius; set => _radius = value; }

        public BattleLineNode(float radius)
        {
            _radius = radius;
        }

        public BattleLineNode()
        {
            _radius = 1;
        }
    }
}