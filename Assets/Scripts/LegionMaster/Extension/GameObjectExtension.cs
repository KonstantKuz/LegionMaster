using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace LegionMaster.Extension
{
    [PublicAPI]
    public static class GameObjectExtension
    {
        public static T[] GetComponentsOnlyInChildren<T>(this GameObject gameObject, bool includeNotActive = false)
                where T : Component
        {
            return gameObject.GetComponentsInChildren<T>(includeNotActive)
                             .Where(componentsInChild => componentsInChild.gameObject != gameObject)
                             .ToArray();
        }

        public static void SetLayerRecursively(this GameObject go, int layerNumber)
        {
            foreach (var trans in go.GetComponentsInChildren<Transform>(true)) {
                trans.gameObject.layer = layerNumber;
            }
        }

        public static T GetOrCreateComponent<T>(this GameObject gameObject)
                where T : Component
        {
            return (T) GetOrCreateComponent(gameObject, typeof(T));
        }

        public static Component GetOrCreateComponent(this GameObject gameObject, Type componentType)
        {
            return gameObject.GetComponent(componentType) ?? gameObject.AddComponent(componentType);
        }
    }
}