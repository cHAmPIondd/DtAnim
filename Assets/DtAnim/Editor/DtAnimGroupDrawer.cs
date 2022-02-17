using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DtAnim
{
    [CustomPropertyDrawer(typeof(DtAnimGroup))]
    public class DtAnimGroupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.indentLevel++;
            GUILayout.BeginVertical(GUI.skin.window);
            var animsProperty = property.FindPropertyRelative("dtAnims");
            EditorGUILayout.PropertyField(animsProperty);
            GUILayout.EndVertical();
            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }
    }
}