using System;
using UnityDebugSheet.Runtime.Foundation.Drawer;
using UnityEditor;
using UnityEngine;

namespace Drawer.Editor
{
    [CustomEditor(typeof(StatefulDrawer), true)]
    public sealed class StatefulDrawerEditor : DrawerEditor
    {
        private SerializedProperty _maxProgressProp;
        private SerializedProperty _middleProgressProp;
        private SerializedProperty _minProgressProp;
        private SerializedProperty _useMiddleStateProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            _minProgressProp = serializedObject.FindProperty("_minProgress");
            _useMiddleStateProp = serializedObject.FindProperty("_useMiddleState");
            _middleProgressProp = serializedObject.FindProperty("_middleProgress");
            _maxProgressProp = serializedObject.FindProperty("_maxProgress");
        }

        protected override void DrawProperties()
        {
            base.DrawProperties();

            EditorGUILayout.LabelField("States");
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Min", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(_minProgressProp, new GUIContent("Progress"));
                    if (ccs.changed)
                    {
                        _minProgressProp.floatValue =
                            Mathf.Min(_minProgressProp.floatValue, _maxProgressProp.floatValue);
                        if (_useMiddleStateProp.boolValue)
                            _minProgressProp.floatValue =
                                Mathf.Min(_minProgressProp.floatValue, _middleProgressProp.floatValue);
                    }
                }
            }

            EditorGUILayout.LabelField("Middle", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(_useMiddleStateProp, new GUIContent("Enabled"));
                GUI.enabled = _useMiddleStateProp.boolValue;
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(_middleProgressProp, new GUIContent("Progress"));
                    if (ccs.changed)
                    {
                        _middleProgressProp.floatValue =
                            Mathf.Max(_minProgressProp.floatValue, _middleProgressProp.floatValue);
                        _middleProgressProp.floatValue =
                            Mathf.Min(_middleProgressProp.floatValue, _maxProgressProp.floatValue);
                    }
                }

                GUI.enabled = true;
            }

            EditorGUILayout.LabelField("Max", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(_maxProgressProp, new GUIContent("Progress"));
                    if (ccs.changed)
                    {
                        _maxProgressProp.floatValue =
                            Mathf.Max(_minProgressProp.floatValue, _maxProgressProp.floatValue);
                        if (_useMiddleStateProp.boolValue)
                            _maxProgressProp.floatValue = Mathf.Max(_middleProgressProp.floatValue,
                                _maxProgressProp.floatValue);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        protected override void DrawDebugMenu()
        {
            base.DrawDebugMenu();

            var component = (StatefulDrawer)target;
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Set State"))
                {
                    var menu = new GenericMenu();
                    foreach (DrawerState state in Enum.GetValues(typeof(DrawerState)))
                        menu.AddItem(new GUIContent(state.ToString()), false, () =>
                        {
                            component.SetState(state);
                            EditorApplication.QueuePlayerLoopUpdate();
                        });
                    menu.ShowAsContext();
                }

                if (GUILayout.Button("Set Upper State"))
                {
                    component.SetState(component.GetUpperState());
                    EditorApplication.QueuePlayerLoopUpdate();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Set Nearest State"))
                {
                    component.SetState(component.GetNearestState());
                    EditorApplication.QueuePlayerLoopUpdate();
                }

                if (GUILayout.Button("Set Lower State"))
                {
                    component.SetState(component.GetLowerState());
                    EditorApplication.QueuePlayerLoopUpdate();
                }
            }
        }
    }
}
