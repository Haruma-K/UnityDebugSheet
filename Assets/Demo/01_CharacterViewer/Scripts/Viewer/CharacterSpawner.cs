#if !EXCLUDE_UNITY_DEBUG_SHEET
using System.Collections.Generic;
using UnityEngine;

namespace Demo._01_CharacterViewer.Scripts.Viewer
{
    public sealed class CharacterSpawner : MonoBehaviour
    {
        [SerializeField] private CharacterAnimationController[] _prefabs;

        private CharacterAnimationController _activePrefab;

        public IReadOnlyList<CharacterAnimationController> Prefabs => _prefabs;
        public CharacterAnimationController ActiveCharacterAnimationController { get; private set; }

        public void Initialize()
        {
            ChangeCharacter(0);
        }

        public CharacterAnimationController ChangeCharacter(int index)
        {
            if (ActiveCharacterAnimationController != null)
                Destroy(ActiveCharacterAnimationController.gameObject);

            var prefab = _prefabs[index];
            return ChangeCharacter(prefab);
        }

        public CharacterAnimationController ChangeCharacter(CharacterAnimationController prefab)
        {
            var instance = Instantiate(prefab, transform);
            var component = instance.GetComponent<CharacterAnimationController>();
            ActiveCharacterAnimationController = component;
            _activePrefab = prefab;
            component.Initialize();
            Debug.Log("Character changed to " + prefab.name);
            return component;
        }

        /// <summary>
        ///     Get the index of the active character controller.
        /// </summary>
        /// <returns>Index. If the active character controller not exists, return -1.</returns>
        public int GetActiveCharacterControllerIndex()
        {
            for (var i = 0; i < _prefabs.Length; i++)
                if (_prefabs[i] == _activePrefab)
                    return i;

            return -1;
        }
    }
}
#endif
