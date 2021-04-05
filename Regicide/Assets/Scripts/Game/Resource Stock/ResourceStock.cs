
using Mirror;
using System.Collections.Generic;

namespace Regicide.Game.GameResources
{
    public class ResourceStock : NetworkBehaviour
    {
        protected Dictionary<uint, ResourceItem> _resources = new Dictionary<uint, ResourceItem>();
        protected List<ResourceRateModifier> _resourceRates = new List<ResourceRateModifier>();
        private static HashSet<ResourceStock> _resourceStocks = new HashSet<ResourceStock>();

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
        public static IReadOnlyCollection<ResourceStock> ResourceStocks { get => _resourceStocks; }

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

        public virtual void AddResourceRate(ResourceRateModifier resourceRate) => _resourceRates.Add(resourceRate);
        public virtual void RemoveResourceRate(ResourceRateModifier resourceRate) => _resourceRates.Remove(resourceRate);

        public static void UpdateResourceStocks()
        {
            foreach (ResourceStock stock in _resourceStocks)
            {
                List<ResourceRateModifier> resourceRates = stock._resourceRates;
                for (int rateIndex = 0; rateIndex < resourceRates.Count; rateIndex++)
                {
                    ResourceRateModifier rateModifier = resourceRates[rateIndex];
                    rateModifier.UpdateResourceAmount(stock);
                }
            }
        }

        protected virtual void Awake()
        {
            _resourceStocks.Add(this);
        }

        protected virtual void OnDestroy()
        {
            _resourceStocks.Remove(this);
        }
    }
}