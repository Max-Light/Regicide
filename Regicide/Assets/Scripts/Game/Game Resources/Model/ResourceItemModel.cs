
using UnityEngine;

namespace Regicide.Game.GameResources
{
    public class ResourceItemModel 
    {
        public uint ResourceId { get; private set; }
        public string Resource { get; private set; }
        public string Description { get; private set; }
        public Sprite Sprite { get; private set; }

        public ResourceItemModel(uint resourceId, string resourceName, string resourceDescription, Sprite resourceSprite)
        {
            ResourceId = resourceId;
            Resource = resourceName;
            Description = resourceDescription;
            Sprite = resourceSprite;
        }
    }
}