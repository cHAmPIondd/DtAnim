using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace DtAnim
{
    public class DtAnimPresetManager : ScriptableObject
    {
        private static DtAnimPresetManager m_instance;
        public static DtAnimPresetManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = Resources.Load<DtAnimPresetManager>("DtAnimPresetManager");
                    m_instance.Init();
                }
                return m_instance;
            }
        }
        public List<DtAnimPresetCategory> categorys = new List<DtAnimPresetCategory>();
        private string m_storedPath;
        private void Init()
        {
#if UNITY_EDITOR
            UnityEditor.MonoScript script = UnityEditor.MonoScript.FromScriptableObject(m_instance);
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(script);
            var assetName = Path.GetFileName(assetPath);
            var assetDir = assetPath.Substring(0, assetPath.Length - assetName.Length);
            m_storedPath = Path.Combine(assetDir, "Resources", "Presets");
#endif
        }
        public DtAnim LoadPreset(string _categoryName, string _presetName)
        {
            var preset = categorys.Where(x => x.categoryName == _categoryName)
                     .Select(x => x.GetPreset(_presetName));
            if (preset.Count() > 0)
                return preset.FirstOrDefault().dtAnim.Clone() as DtAnim;
            return new DtAnim();
        }
#if UNITY_EDITOR
        public List<string> GetCatergoryNames()
        {
            return categorys.Select(x => x.name).ToList();
        }
        public List<string> GetPresetNames(string _categoryName)
        {
            return categorys.Where(x => x.categoryName == _categoryName)
                            .SelectMany(x => x.presets.Select(x => x.presetName))
                            .ToList();
        }
        public bool IsExistCategory(string _categoryName)
        {
            var category = GetCategory(_categoryName, false);
            return category != null;
        }

        public bool IsExistPreset(string _categoryName, string _presetName)
        {
            var category = GetCategory(_categoryName, false);
            if (category == null) return false;
            return category.IsExistPreset(_presetName);
        }
        public void CreateNewPreset(string _categoryName, string _presetName, DtAnim _dtAnim)
        {
            var category = GetCategory(_categoryName, true);
            category.CreateNewPreset(_presetName, _dtAnim);
        }
        public void UpdatePreset(string _categoryName, string _presetName, DtAnim _dtAnim)
        {
            var category = GetCategory(_categoryName, false);
            if (category != null)
            {
                category.UpdatePreset(_presetName, _dtAnim);
            }
        }
        public void DeletePreset(string _categoryName, string _presetName)
        {
            var category = GetCategory(_categoryName, false);
            if (category != null)
            {
                category.DeletePreset(_presetName);
                if (category.presets.Count == 0)
                {
                    categorys.Remove(category);
                    DestroyImmediate(category, true);
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.AssetDatabase.SaveAssets();
                }
            }
        }
        private DtAnimPresetCategory GetCategory(string _categoryName, bool createWhenNotFound)
        {
            var category = categorys.Find(x => x.categoryName == _categoryName);
            if (category == null && createWhenNotFound)
            {
                category = CreateNewCategory(_categoryName);
                categorys.Add(category);
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            return category;
        }
        private DtAnimPresetCategory CreateNewCategory(string _categoryName)
        {
            var newCatergory = CreateInstance<DtAnimPresetCategory>();
            newCatergory.categoryName = _categoryName;
            UnityEditor.AssetDatabase.CreateAsset(newCatergory, Path.Combine(m_storedPath, _categoryName + ".asset"));
            return newCatergory;
        }
#endif
    }
}