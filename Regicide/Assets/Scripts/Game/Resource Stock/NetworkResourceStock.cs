using Mirror;
using System.Collections.Generic;

namespace Regicide.Game.GameResources
{
    public class NetworkResourceStock : ResourceStock
    {
        private struct ResourceRate 
        {
            public uint resourceId;
            public float rate;
            public string source;
        }

        private class SyncResourceAmount : SyncDictionary<uint, float> { }
        private SyncResourceAmount _syncResource = new SyncResourceAmount();

        private class SyncResourceRate : SyncList<ResourceRate> { }
        private SyncResourceRate _syncResourceRates = new SyncResourceRate();

        public static Dictionary<uint, NetworkResourceStock> NetResourceStocks { get; private set; } = new Dictionary<uint, NetworkResourceStock>();

        [Server]
        public override void RegisterResource(ResourceItem resource)
        {
            base.RegisterResource(resource);
            _syncResource.Add(resource.Model.ResourceId, resource.Amount);
            resource.AddObserver(() => SynchronizeResource(resource));
        }

        [Server]
        public override void UnregisterResource(ResourceItem resource)
        {
            base.UnregisterResource(resource);
            _syncResource.Remove(resource.Model.ResourceId);
            resource.RemoveObserver(() => SynchronizeResource(resource));
        }

        [Server]
        public override void AddResourceRate(ResourceRateModifier resourceRate)
        {
            _syncResourceRates.Add(new ResourceRate
            {
                resourceId = resourceRate.Resource.Model.ResourceId,
                rate = resourceRate.Rate,
                source = resourceRate.Source
            });
            resourceRate.AddObserver(() => SynchronizeResourceRate(resourceRate));
            base.AddResourceRate(resourceRate);
        }

        [Server]
        public override void RemoveResourceRate(ResourceRateModifier resourceRate)
        {
            int index = _resourceRates.IndexOf(resourceRate);
            if (index > 0)
            {
                _syncResourceRates.RemoveAt(index);
                resourceRate.RemoveObserver(() => SynchronizeResourceRate(resourceRate));
                base.RemoveResourceRate(resourceRate);
            }
        }

        private void OnResourceAmountChange(SyncResourceAmount.Operation op, uint resourceId, float amount)
        {
            if (isServer) { return; }
            switch (op)
            {
                case SyncIDictionary<uint, float>.Operation.OP_ADD:
                    ResourceItem resource = ResourceItemFactory.GetResource(resourceId);
                    resource.Amount = amount;
                    _resources.Add(resourceId, resource);
                    break;
                case SyncIDictionary<uint, float>.Operation.OP_REMOVE:
                    _resources.Remove(resourceId);
                    break;
                case SyncIDictionary<uint, float>.Operation.OP_SET:
                    SynchronizeResource(resourceId, amount);
                    break;
            }
        }

        private void OnResourceRateChange(SyncResourceRate.Operation op, int index, ResourceRate _, ResourceRate resourceRate)
        {
            if (isServer) { return; }
            switch (op)
            {
                case SyncList<ResourceRate>.Operation.OP_ADD:
                    _resourceRates.Add(new ResourceRateModifier(_resources[resourceRate.resourceId], resourceRate.rate, resourceRate.source));
                    break;
                case SyncList<ResourceRate>.Operation.OP_REMOVEAT:
                    _resourceRates.RemoveAt(index);
                    break;
                case SyncList<ResourceRate>.Operation.OP_SET:
                    _resourceRates[index].Rate = resourceRate.rate;
                    break;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _syncResource.Callback += OnResourceAmountChange;
            _syncResourceRates.Callback += OnResourceRateChange;

            if (isServer) { return; }
            foreach (KeyValuePair<uint, float> resource in _syncResource)
            {
                ResourceItem resourceItem = ResourceItemFactory.GetResource(resource.Key);
                resourceItem.Amount = resource.Value;
                _resources.Add(resource.Key, resourceItem);
            }
            for (int rateIndex = 0; rateIndex < _syncResourceRates.Count; rateIndex++)
            {
                ResourceRate resourceRate = _syncResourceRates[rateIndex];
                _resourceRates.Add(new ResourceRateModifier(_resources[resourceRate.resourceId], resourceRate.rate, resourceRate.source));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _syncResource.Callback -= OnResourceAmountChange;
            _syncResourceRates.Callback -= OnResourceRateChange;
        }

        private void SynchronizeResource(uint resourceId, float amount)
        {
            ResourceItem resource = _resources[resourceId];
            if (resource != null)
            {
                resource.Amount = amount;
            }
        }

        [Server]
        private void SynchronizeResource(ResourceItem resource)
        {
            _syncResource[resource.Model.ResourceId] = resource.Amount;
        }

        [Server]
        private void SynchronizeResourceRate(ResourceRateModifier resourceRate)
        {
            int index = _resourceRates.IndexOf(resourceRate);
            if (index > 0)
            {
                _syncResourceRates[index] = new ResourceRate
                {
                    resourceId = _syncResourceRates[index].resourceId,
                    rate = resourceRate.Rate,
                    source = _syncResourceRates[index].source
                };
            }
        }
    }
}