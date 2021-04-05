
using Mirror;
using System.Collections.Generic;

namespace Regicide.Game.GameResources
{
    public class ResourceStock : NetworkBehaviour
    {
        protected Dictionary<uint, ResourceItem> _resources = new Dictionary<uint, ResourceItem>();
        protected List<ResourceRateModifier> _resourceRates = new List<ResourceRateModifier>();

        public ResourceItem this[uint key]
        {
            get
            {
                if (_resources.TryGetValue(key, out ResourceItem resource))
                {
                    return resource;
                }
                return null;
            }
        }

        public IReadOnlyList<IResourceRate> ResourceRates { get => _resourceRates; }

        public virtual void RegisterResource(ResourceItem resource) => _resources.Add(resource.Model.ResourceId, resource);
        public virtual void UnregisterResource(ResourceItem resource) => _resources.Remove(resource.Model.ResourceId);

        public List<ResourceItem> GetResourcesOfType<T>() where T : IResourceType
        {
            List<ResourceItem> resourcesOfType = new List<ResourceItem>();
            foreach (ResourceItem resource in _resources.Values)
            {
                if (resource is T)
                {
                    resourcesOfType.Add(resource);
                }
            }
            return resourcesOfType;
        }

        public List<IResourceRate> GetRatesOfResource<T>() where T : ResourceItem
        {
            List<IResourceRate> resourceRates = new List<IResourceRate>();
            for (int rateIndex = 0; rateIndex < resourceRates.Count; rateIndex++)
            {
                if (_resourceRates[rateIndex].Resource is T)
                {
                    resourceRates.Add(_resourceRates[rateIndex]);
                }
            }
            return resourceRates;
        }

        public virtual void SetResourceRate(IResourceRate rateModifier, float rate)
        {
            ResourceRateModifier resourceRate = rateModifier as ResourceRateModifier;
            resourceRate.Rate = rate;
        }

        public virtual IResourceRate CreateResourceRate(ResourceRateBuilder rateBuilder)
        {
            ResourceRateModifier rateModifier = rateBuilder;
            if (rateModifier != null)
            {
                _resourceRates.Add(rateModifier);
                return rateModifier;
            }
            return null;
        }

        public virtual void RemoveResourceRate(IResourceRate rateModifier) => _resourceRates.Remove(rateModifier as ResourceRateModifier);
    }
}