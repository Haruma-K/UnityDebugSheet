using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    public sealed class PrefabContainer : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _prefabs = new List<GameObject>();

        private readonly Dictionary<string, GameObject> _prefabNames = new Dictionary<string, GameObject>();

        public List<GameObject> Prefabs => _prefabs;

        public GameObject GetPrefab(string prefabName)
        {
            if (_prefabNames.TryGetValue(prefabName, out var prefab))
                return prefab;

            prefab = _prefabs.FirstOrDefault(x => x.name.Equals(prefabName, StringComparison.Ordinal));

            if (prefab == null)
                throw new ArgumentException($"Prefab \"{prefabName}\" is not found.");

            _prefabNames.Add(prefabName, prefab);
            return prefab;
        }

        public bool TryGetPrefab(string prefabName, out GameObject prefab)
        {
            try
            {
                prefab = GetPrefab(prefabName);
                return true;
            }
            catch
            {
                prefab = null;
                return false;
            }
        }
    }
}
