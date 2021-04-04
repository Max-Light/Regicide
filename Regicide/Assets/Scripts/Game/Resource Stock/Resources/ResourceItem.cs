
using System.Collections.Generic;

namespace Regicide.Game.GameResources
{
    public abstract class ResourceItem : IResourceType
    {
        public float Amount { get; set; } = 0;
        public virtual ResourceItemModel Model => null;
    }
}