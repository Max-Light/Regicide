using Regicide.Game.GameResources;
using UnityEngine;

public class GameResourceFactoryInitializer : MonoBehaviour
{
    private void Awake()
    {
        ResourceItemFactory.InitializeFactory();
    }
}
