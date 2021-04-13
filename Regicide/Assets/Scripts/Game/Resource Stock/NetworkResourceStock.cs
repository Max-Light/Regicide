using Mirror;
using System;
using System.Collections.Generic;

namespace Regicide.Game.GameResources
{
    public class NetworkResourceStock : ResourceStock
    {
        private struct SyncResourceRate 
        {
            public uint resourceId;
            public float rate;
            public string source;
        }

        private class SyncResourceAmountStock : SyncDictionary<uint, float> { }
        private SyncResourceAmountStock _syncResource = new SyncResourceAmountStock();

        private class SyncResourceRateStock : SyncList<SyncResourceRate> { }
        private SyncResourceRateStock _syncResourceRates = new SyncResourceRateStock();

        private Dictionary<ResourceItem, Action> _syncResourceChangeActions = new Dictionary<ResourceItem, Action>();
        private Dictionary<ResourceRateModifier, Action> _syncResourceRateChangeActions = new Dictionary<ResourceRateModifier, Action>();

        public static Dictionary<uint, NetworkResourceStock> NetResourceStocks { get; private set; } = new Dictionary<uint, NetworkResourceStock>();

        [Server]
        public override void RegisterResource(ResourceItem resource)
        {
            _syncResource.Add(resource.Model.ResourceId, resource.Amount);
            Action syncResourceAction = () => SynchronizeResource(resource);
            resource.AddObserver(syncResourceAction);
            _syncResourceChangeActions.Add(resource, syncResourceAction);
            base.RegisterResource(resource);
        }

        [Server]
        public override void UnregisterResource(ResourceItem resource)
        {
            _syncResource.Remove(resource.Model.ResourceId);
            resource.RemoveObserver(_syncResourceChangeActions[resource]);
            _syncResourceChangeActions.Remove(resource);
            base.UnregisterResource(resource);
        }

        [Server]
        public override void AddResourceRate(ResourceRateModifier resourceRate)
        {
            _syncResourceRates.Add(new SyncResourceRate
            {
                resourceId = resourceRate.Resource.Model.ResourceId,
                rate = resourceRate.Rate,
                source = resourceRate.Source
            });
            Action syncResourceRateAction = () => SynchronizeResourceRate(resourceRate);
            resourceRate.AddObserver(syncResourceRateAction);
            _syncResourceRateChangeActions.Add(resourceRate, syncResourceRateAction);
            base.AddResourceRate(resourceRate);
        }

        [Server]
        public override void RemoveResourceRate(ResourceRateModifier resourceRate)
        {
            int index = _resourceRates.IndexOf(resourceRate);
            if (index >= 0)
            {
                _syncResourceRates.RemoveAt(index);
                resourceRate.RemoveObserver(_syncResourceRateChangeActions[resourceRate]);
                _syncResourceRateChangeActions.Remove(resourceRate);
                base.RemoveResourceRate(resourceRate);
            }
        }

        private void OnResourceAmountChange(SyncResourceAmountStock.Operation op, uint resourceId, float amount)
        {
            if (isServer) { return; }
            switch (op)
            {
                case SyncIDictionary<uint, float>.Operation.OP_ADD:
                    {
                        ResourceItem resource = ResourceItemFactory.GetResource(resourceId);
                        resource.Amount = amount;
                        _resources.Add(resourceId, resource);
                        break;
                    }
                case SyncIDictionary<uint, float>.Operation.OP_REMOVE:
                    {
                        _resources.Remove(resourceId);
                        break;
                    }
                case SyncIDictionary<uint, float>.Operation.OP_SET:
                    {
                        ResourceItem resource = _resources[resourceId];
                        if (resource != null)
                        {
                            resource.Amount = amount;
                        }
                        break;
                    }
            }
        }

        private void OnResourceRateChange(SyncResourceRateStock.Operation op, int index, SyncResourceRate _, SyncResourceRate resourceRate)
        {
            if (isServer) { return; }
            switch (op)
            {
                case SyncList<SyncResourceRate>.Operation.OP_ADD:
                    _resourceRates.Add(new ResourceRateModifier(_resources[resourceRate.resourceId], resourceRate.rate, resourceRate.source));
                    break;
                case SyncList<SyncResourceRate>.Operation.OP_REMOVEAT:
                    _resourceRates.RemoveAt(index);
                    break;
                case SyncList<SyncResourceRate>.Operation.OP_SET:
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
                SyncResourceRate resourceRate = _syncResourceRates[rateIndex];
                _resourceRates.Add(new ResourceRateModifier(_resources[resourceRate.resourceId], resourceRate.rate, resourceRate.source));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _syncResource.Callback -= OnResourceAmountChange;
            _syncResourceRates.Callback -= OnResourceRateChange;
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
                _syncResourceRates[index] = new SyncResourceRate
                {
                    resourceId = _syncResourceRates[index].resourceId,
                    rate = resourceRate.Rate,
                    source = _syncResourceRates[index].source
                };
            }
        }
    }
}