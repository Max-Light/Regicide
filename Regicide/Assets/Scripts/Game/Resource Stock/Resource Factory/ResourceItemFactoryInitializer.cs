using Regicide.Game.GameResources;
using UnityEngine;

public class ResourceItemFactoryInitializer : MonoBehaviour
{
    private void Awake()
    {
        ResourceItemFactory.InitializeFactory();
    }

    private void OnDestroy()
    {
        ResourceItemFactory.ClearFactory();
    }
}
