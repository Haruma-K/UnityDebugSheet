using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEditor;
using UnityEngine;

namespace UnityDebugSheet.Editor.Core.Scripts
{
    [CustomPropertyDrawer(typeof(KeyboardShortcut))]
    internal class KeyboardShortcutDrawer : PropertyDrawer
    {
        private readonly Dictionary<string, PropertyData> _propertyDataPerPropertyPath =
            new Dictionary<string, PropertyData>();

        private PropertyData _property;

        private void Init(SerializedProperty property)
        {
            if (_propertyDataPerPropertyPath.TryGetValue(property.propertyPath, out _property)) return;

            _property = new PropertyData();
            _property.enabledProperty = property.FindPropertyRelative("_enabled");
            _property.controlProperty = property.FindPropertyRelative("_control");
            _property.altProperty = property.FindPropertyRelative("_alt");
            _property.shiftProperty = property.FindPropertyRelative("_shift");
            _property.keyProperty = property.FindPropertyRelative("_key");
            _propertyDataPerPropertyPath.Add(property.propertyPath, _property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);

            var fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;

            using (new EditorGUI.PropertyScope(fieldRect, label, property))
            {
                property.isExpanded = EditorGUI.Foldout(new Rect(fieldRect), property.isExpanded, label, true);
                if (property.isExpanded)
                    using (new EditorGUI.IndentLevelScope())
                    {
                        // Enabled
                        fieldRect.y += EditorGUIUtility.standardVerticalSpacing;
                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(new Rect(fieldRect), _property.enabledProperty);

                        // Control
                        fieldRect.y += EditorGUIUtility.standardVerticalSpacing;
                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(new Rect(fieldRect), _property.controlProperty, new GUIContent("Control / Command"));

                        // Alt
                        fieldRect.y += EditorGUIUtility.standardVerticalSpacing;
                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(new Rect(fieldRect), _property.altProperty, new GUIContent("Alt / Option"));

                        // Shift
                        fieldRect.y += EditorGUIUtility.standardVerticalSpacing;
                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(new Rect(fieldRect), _property.shiftProperty);

                        // Key
                        fieldRect.y += EditorGUIUtility.standardVerticalSpacing;
                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(new Rect(fieldRect), _property.keyProperty);
                    }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Init(property);

            var height = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded)
                height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 5;
            return height;
        }

        private class PropertyData
        {
            public SerializedProperty altProperty;
            public SerializedProperty controlProperty;
            public SerializedProperty enabledProperty;
            public SerializedProperty shiftProperty;
            public SerializedProperty keyProperty;
        }
    }
}
