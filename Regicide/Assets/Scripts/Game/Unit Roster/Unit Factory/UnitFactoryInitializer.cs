using UnityEngine;

namespace Regicide.Game.Units
{
    public class UnitFactoryInitializer : MonoBehaviour
    {
        private void Awake()
        {
            UnitFactory.InitializeFactory();
        }

        private void OnDestroy()
        {
            UnitFactory.ClearFactory();
        }
    }
}