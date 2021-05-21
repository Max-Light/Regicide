
using UnityEngine;

namespace Regicide.Game.GameResources
{
    public class BreadResource : ResourceItem, IFoodResourceType
    {
        public static ResourceItemModel ResourceModel { get; private set; } = new ResourceItemModel
            (
            1,
            "Bread",
            "A food source to eat.",
            null
            );

        public override ResourceItemModel Model => ResourceModel;
    }
}