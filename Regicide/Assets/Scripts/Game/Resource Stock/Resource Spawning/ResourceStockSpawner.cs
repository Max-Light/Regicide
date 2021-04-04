
using Mirror;
using UnityEngine;

namespace Regicide.Game.GameResources
{
    [RequireComponent(typeof(ResourceStock))]
    public abstract class ResourceStockSpawner : NetworkBehaviour
    {
        [SerializeField] private ResourceStock _resourceStock = null;

        protected virtual void SpawnResources() { }

        protected void SpawnResource(uint resourceId, float amount = 0)
        {
            ResourceItem resourceItem = ResourceItemFactory.GetResource(resourceId);
            resourceItem.Amount = amount;
            _resourceStock.RegisterResource(resourceItem);
        }

        private void OnValidate()
        {
            _resourceStock = GetComponent<ResourceStock>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SpawnResources();
            if (!isClient)
            {
                Destroy(this);
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Destroy(this);
        }
    }
}