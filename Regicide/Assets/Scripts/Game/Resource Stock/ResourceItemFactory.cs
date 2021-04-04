using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Regicide.Game.GameResources
{
    public static class ResourceItemFactory
    {
        private static Dictionary<uint, Type> _resources = new Dictionary<uint, Type>();

        public static void InitializeFactory()
        {
            ClearFactory();
            var resourceItemTypes = Assembly.GetAssembly(typeof(ResourceItem)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(ResourceItem)));

            foreach (Type type in resourceItemTypes)
            {
                ResourceItem resource = Activator.CreateInstance(type) as ResourceItem;
                ResourceItemModel model = resource.Model;
                if (model != null)
                {
                    if (!_resources.ContainsKey(model.ResourceId))
                    {
                        _resources.Add(model.ResourceId, type);
                        Debug.Log("Registered " + type);
                    }
                    else
                    {
                        Debug.LogError("Could not successfully register Resource: " + resource.ToString() + ", with resource ID " + model.ResourceId);
                    }
                }
                else
                {
                    Debug.LogError(type + " does not have a serialized Resource Profile");
                }
            }
        }

        public static void ClearFactory() => _resources.Clear();

        public static ResourceItem GetResource(uint resourceId)
        {
            if (_resources.TryGetValue(resourceId, out Type resourceType))
            {
                return Activator.CreateInstance(resourceType) as ResourceItem;
            }
            return null;
        }

        public static List<ResourceItem> GetResourcesOfType<T>() where T : IResourceType
        {
            List<ResourceItem> resources = new List<ResourceItem>();

            var resourceItemTypes = Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(ResourceItem)));

            foreach (Type type in resourceItemTypes)
            {
                resources.Add(Activator.CreateInstance(type) as ResourceItem);
            }

            return resources;
        }
    }
}