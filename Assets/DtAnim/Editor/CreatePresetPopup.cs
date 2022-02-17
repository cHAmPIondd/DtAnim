using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DtAnim
{
    public class CreatePresetPopup : EditorWindow
    {
        public static void Popup(DtAnimGroup _dtAnimGroup, string _initalCategory, System.Action<string, string> _createCallback)
        {
            CreatePresetPopup window = (CreatePresetPopup)EditorWindow.GetWindowWithRect<CreatePresetPopup>(new Rect(Screen.width / 2, Screen.height / 2, 400, 150), true, "Create New Preset", true);
            window.m_dtAnimGroup = _dtAnimGroup;
            window.m_categoryName = _initalCategory;
            window.m_createCallback = _createCallback;
            window.ShowPopup();
        }
        private DtAnimGroup m_dtAnimGroup;
        private string m_categoryName;
        private string m_presetName;
        private System.Action<string, string> m_createCallback;
        void OnGUI()
        {
            GUILayout.BeginVertical(GUI.skin.window);
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                var isExistCategory = DtAnimPresetManager.Instance.IsExistCategory(m_categoryName);
                GUILayout.Label("Category Name" + (isExistCategory ? " (Already exists)" : ""));
                m_categoryName = GUILayout.TextField(m_categoryName);
                GUILayout.EndVertical();

                GUILayout.Space(10);
                GUILayout.BeginVertical();
                var isExistPreset = DtAnimPresetManager.Instance.IsExistPreset(m_categoryName, m_presetName);
                GUILayout.Label("Preset Name" + (isExistPreset ? " (Already exists)" : ""));
                m_presetName = GUILayout.TextField(m_presetName);
                GUILayout.EndVertical();

                GUILayout.Space(10);
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (string.IsNullOrEmpty(m_categoryName) || string.IsNullOrEmpty(m_presetName)
                  || isExistPreset)
                    GUI.enabled = false;
                if (GUILayout.Button("Create", GUILayout.Width(60)))
                {
                    DtAnimPresetManager.Instance.CreateNewPreset(m_categoryName, m_presetName, m_dtAnimGroup);
                    m_createCallback?.Invoke(m_categoryName, m_presetName);
                    Close();
                }
                GUILayout.Space(15);
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();

        }
        private void OnLostFocus()
        {
            Focus();
        }
    }
}