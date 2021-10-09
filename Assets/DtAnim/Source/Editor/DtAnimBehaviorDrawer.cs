using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DG.DOTweenEditor;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace DtAnim
{
    [CustomPropertyDrawer(typeof(DtAnimBehavior))]
    public class DtAnimBehaviorDrawer : PropertyDrawer
    {
        private DtAnimBehavior m_src;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            m_src = EditorHelper.GetTargetObjectOfProperty(property) as DtAnimBehavior;
            EditorGUI.BeginProperty(position, label, property);
            if (m_src.target == null)
            {
                var mono = (property.serializedObject.targetObject as MonoBehaviour);
                if (mono == null)
                {
                    EditorGUILayout.LabelField("DtAnimBehavior only can support in MonoBehaviour");
                    return;
                }
                m_src.target = mono.gameObject;
            }

            EditorGUILayout.BeginHorizontal(GUI.skin.button);
            GUILayout.Space(10);
            m_src.isFoldOut = EditorGUILayout.Foldout(m_src.isFoldOut, new GUIContent(property.displayName), true);
            EditorGUILayout.EndHorizontal();
            if (m_src.isFoldOut)
            {
                Undo.RecordObject(m_src.target, "DOTween Animation");
                DrawPreviewEditGUI();
                GUILayout.Space(10);
                if (m_src.IsTweenCreated)
                {
                    GUILayout.Label("Cannot edit in playing");
                }
                var lastGUIEnable = GUI.enabled;
                GUI.enabled = m_src.IsTweenCreated == false && lastGUIEnable;
                DrawTargetGUI(property);
                GUILayout.Space(10);
                DrawLoadPresetGUI();
                GUILayout.Space(10);
                DrawAnimGUI(property);
                GUI.enabled = lastGUIEnable;
            }
            if (GUI.changed) EditorUtility.SetDirty(m_src.target);
            EditorGUI.EndProperty();
        }
        private void DrawPreviewEditGUI()
        {
            GUILayout.BeginVertical(GUI.skin.window);
            GUILayout.BeginVertical(GUI.skin.box);
            {
                GUILayout.Label("Preview");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Play"))
                {
                    m_src.Reset();
                    m_src.CreateNewTween();
                    DOTweenEditorPreview.PrepareTweenForPreview(m_src.m_tween);
                    DOTweenEditorPreview.Start();
                }
                if (GUILayout.Button("Reset"))
                {
                    m_src.Reset();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
        private void DrawTargetGUI(SerializedProperty property)
        {
            GUILayout.BeginVertical(GUI.skin.window);
            GUILayout.BeginVertical(GUI.skin.box);
            {
                var targetProperty = property.FindPropertyRelative("target");
                EditorGUILayout.ObjectField(targetProperty);
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
        private void DrawLoadPresetGUI()
        {
            GUILayout.BeginVertical(GUI.skin.window);
            GUILayout.BeginVertical(GUI.skin.box);
            {
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                GUILayout.Label("Catergory");
                var categoryNames = DtAnimPresetManager.Instance.GetCatergoryNames();
                categoryNames.Insert(0, "Default");
                var categoryIndex = categoryNames.IndexOf(m_src.categoryName);
                categoryIndex = categoryIndex == -1 ? 0 : categoryIndex;
                categoryIndex = EditorGUILayout.Popup(categoryIndex, categoryNames.ToArray());
                m_src.categoryName = categoryNames[categoryIndex];
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.Label("Preset");
                var presetNames = DtAnimPresetManager.Instance.GetPresetNames(m_src.categoryName);
                presetNames.Insert(0, "None");
                var presetIndex = presetNames.IndexOf(m_src.presetName);
                presetIndex = presetIndex == -1 ? 0 : presetIndex;
                presetIndex = EditorGUILayout.Popup(presetIndex, presetNames.ToArray());
                m_src.presetName = presetNames[presetIndex];
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Create New Preset", GUILayout.MaxWidth(180)))
                {
                    CreatePresetPopup.Popup(m_src.dtAnim, m_src.categoryName,
                        (_newCategoryName, _newPresetName) =>
                        {
                            m_src.categoryName = _newCategoryName;
                            m_src.presetName = _newPresetName;
                        });
                }
                GUILayout.FlexibleSpace();
                var lastGUIEnable = GUI.enabled;
                GUI.enabled = presetIndex != 0 && lastGUIEnable;
                if (GUILayout.Button("Load"))
                {
                    m_src.dtAnim = DtAnimPresetManager.Instance.LoadPreset(m_src.categoryName, m_src.presetName);
                }
                if (GUILayout.Button("Save"))
                {
                    DtAnimPresetManager.Instance.UpdatePreset(m_src.categoryName, m_src.presetName, m_src.dtAnim);
                }
                if (GUILayout.Button("Delete"))
                {
                    var confirmDialog = EditorUtility.DisplayDialog("Delete", $"Preset Category:{m_src.categoryName}\nPreset Name:{m_src.presetName}", "Yes", "No");
                    if (confirmDialog)
                        DtAnimPresetManager.Instance.DeletePreset(m_src.categoryName, m_src.presetName);
                }
                GUILayout.Space(10);
                GUI.enabled = lastGUIEnable;
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                m_src.isLoadPresetAtRuntime = GUILayout.Toggle(m_src.isLoadPresetAtRuntime, new GUIContent("Is load preset at runtime"));
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
        private void DrawAnimGUI(SerializedProperty property)
        {
            var lastGUIEnable = GUI.enabled;
            GUI.enabled = m_src.isLoadPresetAtRuntime == false && lastGUIEnable;
            var dtAnimProperty = property.FindPropertyRelative("dtAnim");
            EditorGUILayout.PropertyField(dtAnimProperty);
            GUI.enabled = lastGUIEnable;
        }
    }
}