using UnityEditor;
using UnityEngine;

namespace Drawer.Editor
{
    [CustomEditor(typeof(UnityDebugSheet.Runtime.Foundation.Drawer.Drawer), true)]
    public class DrawerEditor : UnityEditor.Editor
    {
        private bool _debugFoldout;
        private SerializedProperty _directionProp;
        private SerializedProperty _moveInsideSafeAreaProp;
        private SerializedProperty _openOnStartProp;
        private SerializedProperty _scriptProp;
        private SerializedProperty _sizeProp;

        protected virtual void OnEnable()
        {
            _scriptProp = serializedObject.FindProperty("m_Script");
            _directionProp = serializedObject.FindProperty("_direction");
            _sizeProp = serializedObject.FindProperty("_size");
            _moveInsideSafeAreaProp = serializedObject.FindProperty("_moveInsideSafeArea");
            _openOnStartProp = serializedObject.FindProperty("_openOnStart");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawProperties();
            serializedObject.ApplyModifiedProperties();

            _debugFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_debugFoldout, "Debug");
            if (_debugFoldout)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawDebugMenu();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        protected virtual void DrawProperties()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_scriptProp);
            GUI.enabled = true;
            EditorGUILayout.PropertyField(_directionProp);
            EditorGUILayout.PropertyField(_sizeProp);
            EditorGUILayout.PropertyField(_moveInsideSafeAreaProp);
            EditorGUILayout.PropertyField(_openOnStartProp);
        }

        protected virtual void DrawDebugMenu()
        {
            var component = (UnityDebugSheet.Runtime.Foundation.Drawer.Drawer)target;
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var progress = EditorGUILayout.Slider("Progress", component.Progress, 0.0f, 1.0f);
                if (ccs.changed)
                {
                    component.Progress = progress;
                    EditorApplication.QueuePlayerLoopUpdate();
                }
            }
        }
    }
}
