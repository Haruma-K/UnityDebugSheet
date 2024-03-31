using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    public sealed class PrefabContainer : MonoBehaviour
    {
        // For Inspector
        [SerializeField] private List<GameObject> _prefabs = new List<GameObject>();

        private readonly Dictionary<string, GameObject> _nameToPrefabMap = new Dictionary<string, GameObject>();

        private void Awake()
        {
            foreach (var prefab in _prefabs)
                AddPrefab(prefab);
        }

        public void AddPrefab(GameObject prefab)
        {
            // Add prefab to the map.
            // If the same name prefab is already added, it will be overwritten.
            _nameToPrefabMap[prefab.name] = prefab;
        }

        public GameObject GetPrefab(string prefabName)
        {
            if (!_nameToPrefabMap.TryGetValue(prefabName, out var prefab))
                throw new ArgumentException($"Prefab '{prefabName}' is not found.");

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