using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace DtAnim
{
    public class DtAnimPresetCategory : ScriptableObject
    {
        public string categoryName;
        public List<DtAnimPreset> presets = new List<DtAnimPreset>();
        public DtAnimPreset GetPreset(string _presetName)
        {
            return presets.Find(x => x.presetName == _presetName);
        }
#if UNITY_EDITOR
        public bool IsExistPreset(string _presetName)
        {
            var preset = presets.Find(x => x.presetName == _presetName);
            return preset != null;
        }
        public void CreateNewPreset(string _presetName, DtAnimGroup _dtAnimGroup)
        {
            Assert.IsNull(presets.Find(x => x.presetName == _presetName));
            var newPreset = CreateInstance<DtAnimPreset>();
            newPreset.name = _presetName;
            newPreset.presetName = _presetName;
            newPreset.dtAnimGroup = _dtAnimGroup.Clone();
            presets.Add(newPreset);
            UnityEditor.AssetDatabase.AddObjectToAsset(newPreset, this);
            UnityEditor.EditorUtility.SetDirty(this);
            //UnityEditor.AssetDatabase.SaveAssets();
        }
        public void UpdatePreset(string _presetName, DtAnimGroup _dtAnimGroup)
        {
            var preset = presets.Find(x => x.presetName == _presetName);
            preset.dtAnimGroup = _dtAnimGroup.Clone();
            UnityEditor.EditorUtility.SetDirty(this);
            //UnityEditor.AssetDatabase.SaveAssets();
        }
        public void DeletePreset(string _presetName)
        {
            var preset = presets.Find(x => x.presetName == _presetName);
            if (preset != null)
            {
                presets.Remove(preset);
                UnityEditor.AssetDatabase.RemoveObjectFromAsset(preset);
                UnityEditor.EditorUtility.SetDirty(this);
                //UnityEditor.AssetDatabase.SaveAssets();
            }
        }
#endif
    }
}