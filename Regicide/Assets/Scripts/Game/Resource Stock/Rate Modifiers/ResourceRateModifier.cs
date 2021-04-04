
namespace Regicide.Game.GameResources
{
    public class ResourceRateModifier : IResourceRate
    {
        public ResourceItem Resource { get; private set; }
        public float Rate { get; set; } = 0;
        public string Source { get; private set; } = "Base";

        public ResourceRateModifier(ResourceItem resource, float rate, string source)
        {
            Resource = resource;
            Rate = rate;
            Source = source;
        }

        public virtual void UpdateResourceAmount(ResourceStock resourceStock)
        {
            Resource.Amount += Rate;
        }
    }
}