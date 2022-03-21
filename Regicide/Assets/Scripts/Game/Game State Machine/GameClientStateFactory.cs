using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Regicide.Game.GameStates
{
    public static class GameClientStateFactory
    {
        private static Dictionary<uint, Type> _gameStates = new Dictionary<uint, Type>();

        public static void InitializeStateFactory()
        {
            ClearStateFactory();
            var gameStateTypes = Assembly.GetAssembly(typeof(GameClientState)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(GameClientState)));

            foreach (Type type in gameStateTypes)
            {
                GameClientState stateInstance = Activator.CreateInstance(type) as GameClientState;
                if (!_gameStates.ContainsKey(stateInstance.GameStateId))
                {
                    _gameStates.Add(stateInstance.GameStateId, type);
                }
                else
                {
                    Debug.LogError("Could not successfully register client game state: " + stateInstance.ToString() + " with state ID " + stateInstance.GameStateId);
                }
            }
        }

        public static void ClearStateFactory() => _gameStates.Clear();

        public static GameClientState GetGameState(uint stateId)
        {
            if (_gameStates.TryGetValue(stateId, out Type typeValue))
            {
                return Activator.CreateInstance(typeValue) as GameClientState;
            }
            return null;
        }
    }
}