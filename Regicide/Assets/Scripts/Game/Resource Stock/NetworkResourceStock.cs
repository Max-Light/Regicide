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
        }

        [Server]
        public override void UnregisterResource(ResourceItem resource)
        {
            base.UnregisterResource(resource);
            _syncResource.Remove(resource.Model.ResourceId);
        }

        [Server]
        public override void AddResourceRate(IResourceRate rateModifier)
        {
            ResourceRate rate = new ResourceRate
            {
                resourceId = rateModifier.Resource.Model.ResourceId,
                rate = rateModifier.Rate,
                source = rateModifier.Source
            };
            _syncResourceRates.Add(rate);
            base.AddResourceRate(rateModifier);
        }

        [Server]
        public override void RemoveResourceRate(IResourceRate rateModifier)
        {
            int index = _resourceRates.IndexOf(rateModifier);
            if (index > 0)
            {
                _syncResourceRates.RemoveAt(index);
                base.RemoveResourceRate(rateModifier);
            }
        }

        [Server]
        public void SynchronizeResourceRate(IResourceRate rateModifier, float rate)
        {
            int index = _resourceRates.IndexOf(rateModifier);
            if (index > 0)
            {
                _syncResourceRates[index] = new ResourceRate
                {
                    resourceId = rateModifier.Resource.Model.ResourceId,
                    rate = rate,
                    source = rateModifier.Source
                };
                rateModifier.Rate = rate;
            }
        }

        [Server]
        public static void UpdateResourceStocks()
        {
            foreach (NetworkResourceStock stock in NetResourceStocks.Values)
            {
                List<IResourceRate> resourceRates = stock._resourceRates;
                for (int rateIndex = 0; rateIndex < resourceRates.Count; rateIndex++)
                {
                    IResourceRate resourceRate = resourceRates[rateIndex];
                    resourceRate.UpdateResourceAmount(stock);
                    stock._syncResource[resourceRate.Resource.Model.ResourceId] = resourceRate.Resource.Amount;
                }
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

        private void Awake()
        {
            NetResourceStocks.Add(netId, this);
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

        private void OnDestroy()
        {
            _syncResource.Callback -= OnResourceAmountChange;
            _syncResourceRates.Callback -= OnResourceRateChange;
            NetResourceStocks.Remove(netId);
        }

        private void SynchronizeResource(uint resourceId, float amount)
        {
            ResourceItem resource = _resources[resourceId];
            if (resource != null)
            {
                resource.Amount = amount;
            }
        }
    }
}