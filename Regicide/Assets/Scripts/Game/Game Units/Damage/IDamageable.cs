
namespace Regicide.Game.Units
{
    public interface IDamageable
    {
        float Health { get; }
        void ReceiveDamage(float damage);
        void ReceiveDamage(DamageReport damageReport);
    }
}