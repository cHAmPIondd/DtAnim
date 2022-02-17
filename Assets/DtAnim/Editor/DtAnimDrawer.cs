using System;
using System.Collections.Generic;
using System.IO;
using DG.DOTweenEditor.UI;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.UIElements;

#if DOTWEEN_TMP
    using TMPro;
#endif

namespace DtAnim
{
    [CustomPropertyDrawer(typeof(DtAnim))]
    public class DtAnimDrawer : PropertyDrawer
    {
        static readonly string[] animationType = new[] {
            "None",
            "Move", "LocalMove",
            "Rotate", "LocalRotate",
            "Scale",
            "Color", "Fade",
            "Text",
            "UIWidthHeight",
            "Punch/Position", "Punch/Rotation", "Punch/Scale",
            "Shake/Position", "Shake/Rotation", "Shake/Scale",
            "Camera/Aspect", "Camera/BackgroundColor", "Camera/FieldOfView", "Camera/OrthoSize", "Camera/PixelRect", "Camera/Rect"
        };
        static string[] animationTypeNoSlashes;// AnimationType list without slashes in values
        static string[] AnimationTypeNoSlashes
        {
            get
            {
                if (animationTypeNoSlashes == null)
                {
                    // Convert _AnimationType to _animationTypeNoSlashes
                    int len = animationType.Length;
                    animationTypeNoSlashes = new string[len];
                    for (int i = 0; i < len; ++i)
                    {
                        string a = animationType[i];
                        a = a.Replace("/", "");
                        animationTypeNoSlashes[i] = a;
                    }
                }
                return animationTypeNoSlashes;
            }
        }
        static string[] _datString; // String representation of DOTweenAnimation enum (here for caching reasons)

        private DtAnim m_src;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            m_src = EditorHelper.GetTargetObjectOfProperty(property) as DtAnim;
            EditorGUI.BeginProperty(position, label, property);

            GUILayout.BeginVertical(GUI.skin.box);
            {
                DrawAnimationTypeGUI();

                if (m_src.animationType != DOTweenAnimationType.None)
                {
                    DrawDtAnimDataGUI();
                }
            }
            GUILayout.EndVertical();

            EditorGUI.EndProperty();
        }
        private void DrawAnimationTypeGUI()
        {
            GUILayout.BeginHorizontal();
            DOTweenAnimationType prevAnimType = m_src.animationType;
            m_src.animationType = AnimationToDOTweenAnimationType(animationType[EditorGUILayout.Popup(DOTweenAnimationTypeToPopupId(m_src.animationType), animationType)]);
            GUILayout.EndHorizontal();

            if (prevAnimType != m_src.animationType)
            {
                // Set default optional values based on animation type
                switch (m_src.animationType)
                {
                    case DOTweenAnimationType.Move:
                    case DOTweenAnimationType.LocalMove:
                    case DOTweenAnimationType.Rotate:
                    case DOTweenAnimationType.LocalRotate:
                    case DOTweenAnimationType.Scale:
                        m_src.endValueV3 = Vector3.zero;
                        m_src.endValueFloat = 0;
                        m_src.optionalBool0 = m_src.animationType == DOTweenAnimationType.Scale;
                        break;
                    case DOTweenAnimationType.UIWidthHeight:
                        m_src.endValueV3 = Vector3.zero;
                        m_src.endValueFloat = 0;
                        m_src.optionalBool0 = m_src.animationType == DOTweenAnimationType.UIWidthHeight;
                        break;
                    case DOTweenAnimationType.Color:
                    case DOTweenAnimationType.Fade:
                        m_src.endValueFloat = 0;
                        break;
                    case DOTweenAnimationType.Text:
                        m_src.optionalBool0 = true;
                        break;
                    case DOTweenAnimationType.PunchPosition:
                    case DOTweenAnimationType.PunchRotation:
                    case DOTweenAnimationType.PunchScale:
                        m_src.endValueV3 = m_src.animationType == DOTweenAnimationType.PunchRotation ? new Vector3(0, 180, 0) : Vector3.one;
                        m_src.optionalFloat0 = 1;
                        m_src.optionalInt0 = 10;
                        m_src.optionalBool0 = false;
                        break;
                    case DOTweenAnimationType.ShakePosition:
                    case DOTweenAnimationType.ShakeRotation:
                    case DOTweenAnimationType.ShakeScale:
                        m_src.endValueV3 = m_src.animationType == DOTweenAnimationType.ShakeRotation ? new Vector3(90, 90, 90) : Vector3.one;
                        m_src.optionalInt0 = 10;
                        m_src.optionalFloat0 = 90;
                        m_src.optionalBool0 = false;
                        break;
                    case DOTweenAnimationType.CameraAspect:
                    case DOTweenAnimationType.CameraFieldOfView:
                    case DOTweenAnimationType.CameraOrthoSize:
                        m_src.endValueFloat = 0;
                        break;
                    case DOTweenAnimationType.CameraPixelRect:
                    case DOTweenAnimationType.CameraRect:
                        m_src.endValueRect = new Rect(0, 0, 0, 0);
                        break;
                }
            }
        }
        private void DrawDtAnimDataGUI()
        {
            GUILayout.BeginHorizontal();
            m_src.duration = EditorGUILayout.FloatField("Duration", m_src.duration);
            if (m_src.duration < 0) m_src.duration = 0;
            m_src.isSpeedBased = GUILayout.Toggle(m_src.isSpeedBased, new GUIContent("SpeedBased", "If selected, the duration will count as units/degree x second"), GUILayout.Width(75));
            GUILayout.EndHorizontal();
            m_src.delay = EditorGUILayout.FloatField("Delay", m_src.delay);
            if (m_src.delay < 0) m_src.delay = 0;
            m_src.isIndependentUpdate = EditorGUILayout.Toggle("Ignore TimeScale", m_src.isIndependentUpdate);
            m_src.easeType = EditorGUIUtils.FilteredEasePopup("Ease", m_src.easeType);
            if (m_src.easeType == Ease.INTERNAL_Custom)
            {
                m_src.easeCurve = EditorGUILayout.CurveField("   Ease Curve", m_src.easeCurve);
            }
            m_src.loops = EditorGUILayout.IntField(new GUIContent("Loops", "Set to -1 for infinite loops"), m_src.loops);
            if (m_src.loops < -1) m_src.loops = -1;
            if (m_src.loops > 1 || m_src.loops == -1)
                m_src.loopType = (LoopType)EditorGUILayout.EnumPopup("   Loop Type", m_src.loopType);
            m_src.id = EditorGUILayout.TextField("ID", m_src.id);

            bool canBeRelative = true;
            // End value and eventual specific options
            switch (m_src.animationType)
            {
                case DOTweenAnimationType.Move:
                case DOTweenAnimationType.LocalMove:
                    GUIEndValueV3();
                    m_src.optionalBool0 = EditorGUILayout.Toggle("    Snapping", m_src.optionalBool0);
                    canBeRelative = true;
                    break;
                case DOTweenAnimationType.Rotate:
                case DOTweenAnimationType.LocalRotate:
                    GUIEndValueV3();
                    m_src.optionalRotationMode = (RotateMode)EditorGUILayout.EnumPopup("    Rotation Mode", m_src.optionalRotationMode);
                    break;
                case DOTweenAnimationType.Scale:
                    if (m_src.optionalBool0) GUIEndValueFloat();
                    else GUIEndValueV3();
                    m_src.optionalBool0 = EditorGUILayout.Toggle("Uniform Scale", m_src.optionalBool0);
                    break;
                case DOTweenAnimationType.UIWidthHeight:
                    if (m_src.optionalBool0) GUIEndValueFloat();
                    else GUIEndValueV2();
                    m_src.optionalBool0 = EditorGUILayout.Toggle("Uniform Scale", m_src.optionalBool0);
                    break;
                case DOTweenAnimationType.Color:
                    GUIEndValueColor();
                    canBeRelative = false;
                    break;
                case DOTweenAnimationType.Fade:
                    GUIEndValueFloat();
                    if (m_src.endValueFloat < 0) m_src.endValueFloat = 0;
                    canBeRelative = false;
                    break;
                case DOTweenAnimationType.Text:
                    GUIEndValueString();
                    m_src.optionalBool0 = EditorGUILayout.Toggle("Rich Text Enabled", m_src.optionalBool0);
                    m_src.optionalScrambleMode = (ScrambleMode)EditorGUILayout.EnumPopup("Scramble Mode", m_src.optionalScrambleMode);
                    m_src.optionalString = EditorGUILayout.TextField(new GUIContent("Custom Scramble", "Custom characters to use in case of ScrambleMode.Custom"), m_src.optionalString);
                    break;
                case DOTweenAnimationType.PunchPosition:
                case DOTweenAnimationType.PunchRotation:
                case DOTweenAnimationType.PunchScale:
                    GUIEndValueV3();
                    canBeRelative = false;
                    m_src.optionalInt0 = EditorGUILayout.IntSlider(new GUIContent("    Vibrato", "How much will the punch vibrate"), m_src.optionalInt0, 1, 50);
                    m_src.optionalFloat0 = EditorGUILayout.Slider(new GUIContent("    Elasticity", "How much the vector will go beyond the starting position when bouncing backwards"), m_src.optionalFloat0, 0, 1);
                    if (m_src.animationType == DOTweenAnimationType.PunchPosition) m_src.optionalBool0 = EditorGUILayout.Toggle("    Snapping", m_src.optionalBool0);
                    break;
                case DOTweenAnimationType.ShakePosition:
                case DOTweenAnimationType.ShakeRotation:
                case DOTweenAnimationType.ShakeScale:
                    GUIEndValueV3();
                    canBeRelative = false;
                    m_src.optionalInt0 = EditorGUILayout.IntSlider(new GUIContent("    Vibrato", "How much will the shake vibrate"), m_src.optionalInt0, 1, 50);
                    m_src.optionalFloat0 = EditorGUILayout.Slider(new GUIContent("    Randomness", "The shake randomness"), m_src.optionalFloat0, 0, 90);
                    if (m_src.animationType == DOTweenAnimationType.ShakePosition) m_src.optionalBool0 = EditorGUILayout.Toggle("    Snapping", m_src.optionalBool0);
                    break;
                case DOTweenAnimationType.CameraAspect:
                case DOTweenAnimationType.CameraFieldOfView:
                case DOTweenAnimationType.CameraOrthoSize:
                    GUIEndValueFloat();
                    canBeRelative = false;
                    break;
                case DOTweenAnimationType.CameraBackgroundColor:
                    GUIEndValueColor();
                    canBeRelative = false;
                    break;
                case DOTweenAnimationType.CameraPixelRect:
                case DOTweenAnimationType.CameraRect:
                    GUIEndValueRect();
                    canBeRelative = false;
                    break;
            }

            // Final settings
            if (canBeRelative) m_src.isRelative = EditorGUILayout.Toggle("    Relative", m_src.isRelative);

        }
        #region Methods
        DOTweenAnimationType AnimationToDOTweenAnimationType(string animation)
        {
            if (_datString == null) _datString = Enum.GetNames(typeof(DOTweenAnimationType));
            animation = animation.Replace("/", "");
            return (DOTweenAnimationType)(Array.IndexOf(_datString, animation));
        }
        int DOTweenAnimationTypeToPopupId(DOTweenAnimationType animation)
        {
            return Array.IndexOf(AnimationTypeNoSlashes, animation.ToString());
        }

        #endregion

        #region GUI Draw Methods

        void GUIEndValueFloat()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            m_src.endValueFloat = EditorGUILayout.FloatField(m_src.endValueFloat);
            GUILayout.EndHorizontal();
        }

        void GUIEndValueColor()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            m_src.endValueColor = EditorGUILayout.ColorField(m_src.endValueColor);
            GUILayout.EndHorizontal();
        }

        void GUIEndValueV3()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            m_src.endValueV3 = EditorGUILayout.Vector3Field("", m_src.endValueV3, GUILayout.Height(16));
            GUILayout.EndHorizontal();
        }

        void GUIEndValueV2()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            m_src.endValueV2 = EditorGUILayout.Vector2Field("", m_src.endValueV2, GUILayout.Height(16));
            GUILayout.EndHorizontal();
        }

        void GUIEndValueString()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            m_src.endValueString = EditorGUILayout.TextArea(m_src.endValueString);
            GUILayout.EndHorizontal();
        }

        void GUIEndValueRect()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            m_src.endValueRect = EditorGUILayout.RectField(m_src.endValueRect);
            GUILayout.EndHorizontal();
        }

        void GUIToFromButton()
        {
            if (GUILayout.Button(m_src.isFrom ? "FROM" : "TO", GUILayout.Width(90))) m_src.isFrom = !m_src.isFrom;
            GUILayout.Space(16);
        }

        #endregion
    }
}