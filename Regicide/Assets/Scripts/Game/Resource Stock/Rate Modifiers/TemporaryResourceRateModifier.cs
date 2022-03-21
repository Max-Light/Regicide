
namespace Regicide.Game.GameResources 
{
    public class TemporaryResourceRateModifier : ResourceRateModifier, IResourceRate
    {
        public int ExpirationDays { get; set; } = 0;

        public TemporaryResourceRateModifier(ResourceItem resource, float rate, string source, int expirationDays) : base(resource, rate, source)
        {
            ExpirationDays = expirationDays;
        }

        public override void UpdateResourceAmount(ResourceStock resourceStock)
        {
            base.UpdateResourceAmount(resourceStock);
            ExpirationDays--;
            if (ExpirationDays == 0)
            {
                resourceStock.RemoveResourceRate(this);
            }
        }
    }
}