using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Regicide.Game.Units
{
    public static class UnitArmorFactory
    {
        private static Dictionary<uint, Type> _armory = new Dictionary<uint, Type>();

        public static void InitializeFactory()
        {
            ClearFactory();
            var armorTypes = Assembly.GetAssembly(typeof(TroopUnitArmor)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(TroopUnitArmor)));

            
        }

        public static void ClearFactory() => _armory.Clear();

        public static TroopUnitArmor GetArmor(uint armorId)
        {
            if (_armory.TryGetValue(armorId, out Type armorType))
            {
                return Activator.CreateInstance(armorType) as TroopUnitArmor;
            }
            Debug.LogWarning("Could not successfully retrieve unit armor");
            return null;
        }
    }
}