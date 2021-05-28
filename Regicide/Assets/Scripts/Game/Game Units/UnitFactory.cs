using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Regicide.Game.Units
{
    public static class UnitFactory
    {
        private static Dictionary<uint, Type> _units = new Dictionary<uint, Type>();

        public static void InitializeFactory()
        {
            ClearFactory();
            var unitTypes = Assembly.GetAssembly(typeof(Unit)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Unit)));

            foreach (Type type in unitTypes)
            {
                Unit unit = Activator.CreateInstance(type) as Unit;
                Unit.Model model = unit.UnitModel;
                if (model != null)
                {
                    if (!_units.ContainsKey(model.UnitId))
                    {
                        _units.Add(model.UnitId, type);
                    }
                    else
                    {
                        Debug.LogError("Could not successfully register Unit: " + unit.ToString() + ", with unit ID " + model.UnitId);
                    }
                }
                else
                {
                    Debug.LogError(type + " does not have a serialized Unit Model");
                }
            }
        }

        public static void ClearFactory() => _units.Clear();

        public static Unit GetUnit(uint unitId)
        {
            if (_units.TryGetValue(unitId, out Type unitType))
            {
                return Activator.CreateInstance(unitType) as Unit;
            }
            Debug.LogWarning("Could not successfully retrieve unit");
            return null;
        }
    }
}