#if !EXCLUDE_UNITY_DEBUG_SHEET
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Demo._01_CharacterViewer.Scripts.Viewer
{
    public sealed class CharacterAnimationController : MonoBehaviour
    {
        private const string SlotName = "Slot01";

        [SerializeField] private AnimationClip[] _clips;

        private Animator _animator;
        private AnimatorOverrideController _overrideController;
        public IReadOnlyList<AnimationClip> Clips => _clips;
        public AnimationClip ActiveClip { get; private set; }

        public void Initialize()
        {
            _animator = GetComponent<Animator>();
            var overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController = _animator.runtimeAnimatorController;
            _animator.runtimeAnimatorController = overrideController;
            _overrideController = overrideController;
            var idleClip = _clips.First(x => x.name.Contains("Idle"));
            ChangeClip(idleClip);
        }

        public void ChangeClip(int index)
        {
            var clip = _clips[index];
            ChangeClip(clip);
        }

        public void ChangeClip(AnimationClip clip)
        {
            _overrideController[SlotName] = clip;
            _animator.Play(SlotName, 0, 0);
            ActiveClip = clip;
        }

        /// <summary>
        ///     Get the index of the active clip.
        /// </summary>
        /// <returns>Index. If the active clip not exists, return -1.</returns>
        public int GetActiveClipIndex()
        {
            for (var i = 0; i < _clips.Length; i++)
                if (_clips[i] == ActiveClip)
                    return i;

            return -1;
        }

#if UNITY_EDITOR
        [MenuItem("CONTEXT/CharacterController/Setup")]
        private static void AddEvent(MenuCommand command)
        {
            var component = (CharacterAnimationController)command.context;
            var fbx = PrefabUtility.GetCorrespondingObjectFromOriginalSource(component.gameObject);
            var fbxPath = AssetDatabase.GetAssetPath(fbx);

            // Setup Importer
            var fbxImporter = (ModelImporter)AssetImporter.GetAtPath(fbxPath);
            var clipAnimations = fbxImporter.defaultClipAnimations.ToList();
            foreach (var clipAnimation in clipAnimations)
            {
                clipAnimation.name = clipAnimation.name.Replace("AnimalArmature|", "");
                clipAnimation.loopTime = true;
            }

            fbxImporter.clipAnimations = clipAnimations.ToArray();
            fbxImporter.SaveAndReimport();

            // Setup Component
            var clips = AssetDatabase.LoadAllAssetRepresentationsAtPath(fbxPath).OfType<AnimationClip>();
            component._clips = clips.ToArray();
            EditorUtility.SetDirty(component.gameObject);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
#endif
