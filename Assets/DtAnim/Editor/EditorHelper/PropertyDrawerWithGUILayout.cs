 using UnityEditor;
 using UnityEngine;

/// <summary>
/// This is a trick. To enable propertydrawer to use guilayout
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(Object), true, isFallback = true)]
public class PropertyDrawerWithGUILayout : Editor { }