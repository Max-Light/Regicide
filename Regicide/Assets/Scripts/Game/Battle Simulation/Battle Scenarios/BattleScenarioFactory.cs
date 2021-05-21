using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Regicide.Game.BattleSimulation
{
    public static class BattleScenarioFactory
    {
        private struct BattleScenarioKey
        {
            private Type _battleLineType_1, _battleLineType_2;

            public BattleScenarioKey(Type battleLineType_1, Type battleLineType_2)
            {
                if (battleLineType_1.GUID.CompareTo(battleLineType_2.GUID) < 0)
                {
                    _battleLineType_1 = battleLineType_1;
                    _battleLineType_2 = battleLineType_2;
                }
                else
                {
                    _battleLineType_1 = battleLineType_2;
                    _battleLineType_2 = battleLineType_1;
                }
            }
        }

        private static Dictionary<BattleScenarioKey, Type> _battleScenarios = new Dictionary<BattleScenarioKey, Type>();

        public static void InitializeFactory()
        {
            ClearFactory();
            var battleScenarioTypes = Assembly.GetAssembly(typeof(BattleScenario)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BattleScenario)));

            foreach (Type type in battleScenarioTypes)
            {
                ConstructorInfo[] constructors = type.GetConstructors();
                for (int constructorIndex = 0; constructorIndex < constructors.Length; constructorIndex++)
                {
                    ParameterInfo[] parameters = constructors[constructorIndex].GetParameters();
                    if (parameters.Length >= 2 && HasBattleLineInterface(parameters[0].ParameterType) && HasBattleLineInterface(parameters[1].ParameterType))
                    {
                        BattleScenarioKey key = new BattleScenarioKey(parameters[0].ParameterType, parameters[1].ParameterType);
                        if (!_battleScenarios.ContainsKey(key))
                        {
                            _battleScenarios.Add(key, type);
                        }
                        else
                        {
                            Debug.LogError("Could not add " + type.FullName + " to battle scenario factory");
                            Debug.LogError("A Battle Scenario with the same Battle Line parameters (" + parameters[0].ParameterType + " + " + parameters[1].ParameterType + ") already exists!");
                        }
                    }
                    else
                    {
                        Debug.LogError(type.FullName + " could not be added to battle scenario factory due to invalid parameters");
                    }
                }
            }
        }

        private static bool HasBattleLineInterface(Type type)
        {
            Type[] interfaceTypes = type.GetInterfaces();
            for (int interfaceIndex = 0; interfaceIndex < interfaceTypes.Length; interfaceIndex++)
            {
                if (interfaceTypes[interfaceIndex].Equals(typeof(IBattleLine)))
                {
                    return true;
                }
            }
            return false;
        }

        public static void ClearFactory() => _battleScenarios.Clear();

        public static BattleScenarioBuilder GetBattleScenario(IBattleLine battleLine_1, IBattleLine battleLine_2)
        {
            BattleScenarioKey key = new BattleScenarioKey(battleLine_1.GetType(), battleLine_2.GetType());
            if (!_battleScenarios.TryGetValue(key, out Type battleScenarioType))
            {
                Debug.LogWarning("Could not retreive battle scenario type");
                battleScenarioType = null;
            }
            else 
            {
                ConstructorInfo[] constructors = battleScenarioType.GetConstructors();
                for (int constructorIndex = 0; constructorIndex < constructors.Length; constructorIndex++)
                {
                    ParameterInfo[] parameters = constructors[constructorIndex].GetParameters();
                    if (parameters[1].ParameterType == battleLine_1.GetType() && parameters[0].ParameterType == battleLine_2.GetType())
                    {
                        IBattleLine battleLineCopy_2 = battleLine_2;
                        battleLine_2 = battleLine_1;
                        battleLine_1 = battleLineCopy_2;
                        break;
                    }
                    else if (parameters[0].ParameterType == battleLine_1.GetType() && parameters[1].ParameterType == battleLine_2.GetType())
                    {
                        break;
                    }
                }
            }

            return new BattleScenarioBuilder(battleScenarioType, battleLine_1, battleLine_2);
        }
    }
}