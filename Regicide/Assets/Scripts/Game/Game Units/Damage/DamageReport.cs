
namespace Regicide.Game.Units
{
    public class DamageReport 
    {
        public float Damage { get; private set; } = 0;

        public DamageReport(float damage)
        {
            Damage = damage;
        }
    }
}