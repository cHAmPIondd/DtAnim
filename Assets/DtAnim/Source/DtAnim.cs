using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

#if DOTWEEN_TMP
	using TMPro;
#endif

namespace DtAnim
{
    public enum DOTweenAnimationType
    {
        None = 0,
        Move = 1,
        LocalMove = 2,
        Rotate = 3,
        LocalRotate = 4,
        Scale = 5,
        Color = 6,
        Fade = 7,
        Text = 8,
        PunchPosition = 9,
        PunchRotation = 10,
        PunchScale = 11,
        ShakePosition = 12,
        ShakeRotation = 13,
        ShakeScale = 14,
        CameraAspect = 15,
        CameraBackgroundColor = 16,
        CameraFieldOfView = 17,
        CameraOrthoSize = 18,
        CameraPixelRect = 19,
        CameraRect = 20,
        UIWidthHeight = 21
    }
    public enum TargetType
    {
        Unset = 0,
        Camera = 1,
        CanvasGroup = 2,
        Image = 3,
        Light = 4,
        RectTransform = 5,
        Renderer = 6,
        SpriteRenderer = 7,
        Rigidbody = 8,
        Rigidbody2D = 9,
        Text = 10,
        Transform = 11,
        tk2dBaseSprite = 12,
        tk2dTextMesh = 13,
        TextMeshPro = 14,
        TextMeshProUGUI = 15
    }
    [System.Serializable]
    public class DtAnim : ICloneable
    {
        static readonly Dictionary<DOTweenAnimationType, Type[]> _AnimationTypeToComponent = new Dictionary<DOTweenAnimationType, Type[]>() {
            { DOTweenAnimationType.Move, new[] { typeof(Rigidbody), typeof(Rigidbody2D), typeof(RectTransform), typeof(Transform) } },
            { DOTweenAnimationType.LocalMove, new[] { typeof(Transform) } },
            { DOTweenAnimationType.Rotate, new[] { typeof(Rigidbody), typeof(Rigidbody2D), typeof(Transform) } },
            { DOTweenAnimationType.LocalRotate, new[] { typeof(Transform) } },
            { DOTweenAnimationType.Scale, new[] { typeof(Transform) } },
            { DOTweenAnimationType.Color, new[] { typeof(SpriteRenderer), typeof(Renderer), typeof(Image), typeof(Text), typeof(Light) } },
            { DOTweenAnimationType.Fade, new[] { typeof(SpriteRenderer), typeof(Renderer), typeof(Image), typeof(Text), typeof(CanvasGroup), typeof(Light) } },
            { DOTweenAnimationType.Text, new[] { typeof(Text) } },
            { DOTweenAnimationType.PunchPosition, new[] { typeof(RectTransform), typeof(Transform) } },
            { DOTweenAnimationType.PunchRotation, new[] { typeof(Transform) } },
            { DOTweenAnimationType.PunchScale, new[] { typeof(Transform) } },
            { DOTweenAnimationType.ShakePosition, new[] { typeof(RectTransform), typeof(Transform) } },
            { DOTweenAnimationType.ShakeRotation, new[] { typeof(Transform) } },
            { DOTweenAnimationType.ShakeScale, new[] { typeof(Transform) } },
            { DOTweenAnimationType.CameraAspect, new[] { typeof(Camera) } },
            { DOTweenAnimationType.CameraBackgroundColor, new[] { typeof(Camera) } },
            { DOTweenAnimationType.CameraFieldOfView, new[] { typeof(Camera) } },
            { DOTweenAnimationType.CameraOrthoSize, new[] { typeof(Camera) } },
            { DOTweenAnimationType.CameraPixelRect, new[] { typeof(Camera) } },
            { DOTweenAnimationType.CameraRect, new[] { typeof(Camera) } },
            { DOTweenAnimationType.UIWidthHeight, new[] { typeof(RectTransform) } },
        };
#if DOTWEEN_TK2D
        static readonly Dictionary<DOTweenAnimationType, Type[]> _Tk2dAnimationTypeToComponent = new Dictionary<DOTweenAnimationType, Type[]>() {
            { DOTweenAnimationType.Scale, new[] { typeof(tk2dBaseSprite), typeof(tk2dTextMesh) } },
            { DOTweenAnimationType.Color, new[] { typeof(tk2dBaseSprite), typeof(tk2dTextMesh) } },
            { DOTweenAnimationType.Fade, new[] { typeof(tk2dBaseSprite), typeof(tk2dTextMesh) } },
            { DOTweenAnimationType.Text, new[] { typeof(tk2dTextMesh) } }
        };
#endif
#if DOTWEEN_TMP
        static readonly Dictionary<DOTweenAnimationType, Type[]> _TMPAnimationTypeToComponent = new Dictionary<DOTweenAnimationType, Type[]>() {
            { DOTweenAnimationType.Color, new[] { typeof(TextMeshPro), typeof(TextMeshProUGUI) } },
            { DOTweenAnimationType.Fade, new[] { typeof(TextMeshPro), typeof(TextMeshProUGUI) } },
            { DOTweenAnimationType.Text, new[] { typeof(TextMeshPro), typeof(TextMeshProUGUI) } }
        };
#endif

        public bool isSpeedBased;
        public float delay;
        public float duration = 1;
        public Ease easeType = Ease.OutQuad;
        public AnimationCurve easeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public LoopType loopType = LoopType.Restart;
        public int loops = 1;
        public string id = "";
        public bool isRelative;
        public bool isFrom;
        public bool isIndependentUpdate = false;

        public Component targetComponent;
        public DOTweenAnimationType animationType;

        public float endValueFloat;
        public Vector3 endValueV3;
        public Vector2 endValueV2;
        public Color endValueColor = new Color(1, 1, 1, 1);
        public string endValueString = "";
        public Rect endValueRect = new Rect(0, 0, 0, 0);

        public bool optionalBool0;
        public float optionalFloat0;
        public int optionalInt0;
        public RotateMode optionalRotationMode = RotateMode.Fast;
        public ScrambleMode optionalScrambleMode = ScrambleMode.None;
        public string optionalString;

        public Tween CreateNewTween(GameObject _target)
        {
            Tween tween = null;
            if (Validate(_target) == false)
            {
                Debug.LogWarning("Tween Animation is not valid");
                return tween;
            }
            Assert.IsNotNull(targetComponent);

            var targetType = TypeToDOTargetType(targetComponent.GetType());

            switch (animationType)
            {
                case DOTweenAnimationType.None:
                    break;
                case DOTweenAnimationType.Move:
                    switch (targetType)
                    {
                        case TargetType.RectTransform:
                            tween = ((RectTransform)targetComponent).DOAnchorPos3D(endValueV3, duration, optionalBool0);
                            break;
                        case TargetType.Transform:
                            tween = ((Transform)targetComponent).DOMove(endValueV3, duration, optionalBool0);
                            break;
                        case TargetType.Rigidbody2D:
                            tween = ((Rigidbody2D)targetComponent).DOMove(endValueV3, duration, optionalBool0);
                            break;
                        case TargetType.Rigidbody:
                            tween = ((Rigidbody)targetComponent).DOMove(endValueV3, duration, optionalBool0);
                            break;
                    }
                    break;
                case DOTweenAnimationType.LocalMove:
                    tween = _target.transform.DOLocalMove(endValueV3, duration, optionalBool0);
                    break;
                case DOTweenAnimationType.Rotate:
                    switch (targetType)
                    {
                        case TargetType.Transform:
                            tween = ((Transform)targetComponent).DORotate(endValueV3, duration, optionalRotationMode);
                            break;
                        case TargetType.Rigidbody2D:
                            tween = ((Rigidbody2D)targetComponent).DORotate(endValueV3.z, duration);
                            break;
                        case TargetType.Rigidbody:
                            tween = ((Rigidbody)targetComponent).DORotate(endValueV3, duration, optionalRotationMode);
                            break;
                    }
                    break;
                case DOTweenAnimationType.LocalRotate:
                    tween = _target.transform.DOLocalRotate(endValueV3, duration, optionalRotationMode);
                    break;
                case DOTweenAnimationType.Scale:
                    switch (targetType)
                    {
#if DOTWEEN_TK2D
                case TargetType.tk2dTextMesh:
                    tween = ((tk2dTextMesh)target).DOScale(optionalBool0 ? new Vector3(endValueFloat, endValueFloat, endValueFloat) : endValueV3, duration);
                    break;
                case TargetType.tk2dBaseSprite:
                    tween = ((tk2dBaseSprite)target).DOScale(optionalBool0 ? new Vector3(endValueFloat, endValueFloat, endValueFloat) : endValueV3, duration);
                    break;
#endif
                        default:
                            tween = _target.transform.DOScale(optionalBool0 ? new Vector3(endValueFloat, endValueFloat, endValueFloat) : endValueV3, duration);
                            break;
                    }
                    break;
                case DOTweenAnimationType.UIWidthHeight:
                    tween = ((RectTransform)targetComponent).DOSizeDelta(optionalBool0 ? new Vector2(endValueFloat, endValueFloat) : endValueV2, duration);
                    break;
                case DOTweenAnimationType.Color:
                    isRelative = false;
                    switch (targetType)
                    {
                        case TargetType.SpriteRenderer:
                            tween = ((SpriteRenderer)targetComponent).DOColor(endValueColor, duration);
                            break;
                        case TargetType.Renderer:
                            tween = ((Renderer)targetComponent).material.DOColor(endValueColor, duration);
                            break;
                        case TargetType.Image:
                            tween = ((Image)targetComponent).DOColor(endValueColor, duration);
                            break;
                        case TargetType.Text:
                            tween = ((Text)targetComponent).DOColor(endValueColor, duration);
                            break;
                        case TargetType.Light:
                            tween = ((Light)targetComponent).DOColor(endValueColor, duration);
                            break;
#if DOTWEEN_TK2D
                case TargetType.tk2dTextMesh:
                    tween = ((tk2dTextMesh)target).DOColor(endValueColor, duration);
                    break;
                case TargetType.tk2dBaseSprite:
                    tween = ((tk2dBaseSprite)target).DOColor(endValueColor, duration);
                    break;
#endif
#if DOTWEEN_TMP
                case TargetType.TextMeshProUGUI:
                    tween = ((TextMeshProUGUI)target).DOColor(endValueColor, duration);
                    break;
                case TargetType.TextMeshPro:
                    tween = ((TextMeshPro)target).DOColor(endValueColor, duration);
                    break;
#endif
                    }
                    break;
                case DOTweenAnimationType.Fade:
                    isRelative = false;
                    switch (targetType)
                    {
                        case TargetType.SpriteRenderer:
                            tween = ((SpriteRenderer)targetComponent).DOFade(endValueFloat, duration);
                            break;
                        case TargetType.Renderer:
                            tween = ((Renderer)targetComponent).material.DOFade(endValueFloat, duration);
                            break;
                        case TargetType.Image:
                            tween = ((Image)targetComponent).DOFade(endValueFloat, duration);
                            break;
                        case TargetType.Text:
                            tween = ((Text)targetComponent).DOFade(endValueFloat, duration);
                            break;
                        case TargetType.Light:
                            tween = ((Light)targetComponent).DOIntensity(endValueFloat, duration);
                            break;
                        case TargetType.CanvasGroup:
                            tween = ((CanvasGroup)targetComponent).DOFade(endValueFloat, duration);
                            break;
#if DOTWEEN_TK2D
                case TargetType.tk2dTextMesh:
                    tween = ((tk2dTextMesh)target).DOFade(endValueFloat, duration);
                    break;
                case TargetType.tk2dBaseSprite:
                    tween = ((tk2dBaseSprite)target).DOFade(endValueFloat, duration);
                    break;
#endif
#if DOTWEEN_TMP
                case TargetType.TextMeshProUGUI:
                    tween = ((TextMeshProUGUI)target).DOFade(endValueFloat, duration);
                    break;
                case TargetType.TextMeshPro:
                    tween = ((TextMeshPro)target).DOFade(endValueFloat, duration);
                    break;
#endif
                    }
                    break;
                case DOTweenAnimationType.Text:
                    switch (targetType)
                    {
                        case TargetType.Text:
                            tween = ((Text)targetComponent).DOText(endValueString, duration, optionalBool0, optionalScrambleMode, optionalString);
                            break;
#if DOTWEEN_TK2D
                case TargetType.tk2dTextMesh:
                    tween = ((tk2dTextMesh)target).DOText(endValueString, duration, optionalBool0, optionalScrambleMode, optionalString);
                    break;
#endif
#if DOTWEEN_TMP
                case TargetType.TextMeshProUGUI:
                    tween = ((TextMeshProUGUI)target).DOText(endValueString, duration, optionalBool0, optionalScrambleMode, optionalString);
                    break;
                case TargetType.TextMeshPro:
                    tween = ((TextMeshPro)target).DOText(endValueString, duration, optionalBool0, optionalScrambleMode, optionalString);
                    break;
#endif
                    }
                    break;
                case DOTweenAnimationType.PunchPosition:
                    switch (targetType)
                    {
                        case TargetType.RectTransform:
                            tween = ((RectTransform)targetComponent).DOPunchAnchorPos(endValueV3, duration, optionalInt0, optionalFloat0, optionalBool0);
                            break;
                        case TargetType.Transform:
                            tween = ((Transform)targetComponent).DOPunchPosition(endValueV3, duration, optionalInt0, optionalFloat0, optionalBool0);
                            break;
                    }
                    break;
                case DOTweenAnimationType.PunchScale:
                    tween = _target.transform.DOPunchScale(endValueV3, duration, optionalInt0, optionalFloat0);
                    break;
                case DOTweenAnimationType.PunchRotation:
                    tween = _target.transform.DOPunchRotation(endValueV3, duration, optionalInt0, optionalFloat0);
                    break;
                case DOTweenAnimationType.ShakePosition:
                    switch (targetType)
                    {
                        case TargetType.RectTransform:
                            tween = ((RectTransform)targetComponent).DOShakeAnchorPos(duration, endValueV3, optionalInt0, optionalFloat0, optionalBool0);
                            break;
                        case TargetType.Transform:
                            tween = ((Transform)targetComponent).DOShakePosition(duration, endValueV3, optionalInt0, optionalFloat0, optionalBool0);
                            break;
                    }
                    break;
                case DOTweenAnimationType.ShakeScale:
                    tween = _target.transform.DOShakeScale(duration, endValueV3, optionalInt0, optionalFloat0);
                    break;
                case DOTweenAnimationType.ShakeRotation:
                    tween = _target.transform.DOShakeRotation(duration, endValueV3, optionalInt0, optionalFloat0);
                    break;
                case DOTweenAnimationType.CameraAspect:
                    tween = ((Camera)targetComponent).DOAspect(endValueFloat, duration);
                    break;
                case DOTweenAnimationType.CameraBackgroundColor:
                    tween = ((Camera)targetComponent).DOColor(endValueColor, duration);
                    break;
                case DOTweenAnimationType.CameraFieldOfView:
                    tween = ((Camera)targetComponent).DOFieldOfView(endValueFloat, duration);
                    break;
                case DOTweenAnimationType.CameraOrthoSize:
                    tween = ((Camera)targetComponent).DOOrthoSize(endValueFloat, duration);
                    break;
                case DOTweenAnimationType.CameraPixelRect:
                    tween = ((Camera)targetComponent).DOPixelRect(endValueRect, duration);
                    break;
                case DOTweenAnimationType.CameraRect:
                    tween = ((Camera)targetComponent).DORect(endValueRect, duration);
                    break;
            }

            if (tween == null) return tween;

            if (isFrom)
            {
                ((Tweener)tween).From(isRelative);
            }
            else
            {
                tween.SetRelative(isRelative);
            }
            tween.SetTarget(_target).SetDelay(delay).SetLoops(loops, loopType).SetAutoKill(false);
            if (isSpeedBased) tween.SetSpeedBased();
            if (easeType == Ease.INTERNAL_Custom) tween.SetEase(easeCurve);
            else tween.SetEase(easeType);
            if (!string.IsNullOrEmpty(id)) tween.SetId(id);
            tween.SetUpdate(isIndependentUpdate);

            tween.Pause();

            return tween;
        }
        public void Rewind(Tween _tween)
        {
            if(isFrom)
            {
                _tween.Complete();
            }
            else
            {
                _tween.Rewind();
            }
        }
        #region Internal Static Helpers (also used by Inspector)

        public static TargetType TypeToDOTargetType(Type t)
        {
            string str = t.ToString();
            int dotIndex = str.LastIndexOf(".");
            if (dotIndex != -1) str = str.Substring(dotIndex + 1);
            if (str.IndexOf("Renderer") != -1 && (str != "SpriteRenderer")) str = "Renderer";
            return (TargetType)Enum.Parse(typeof(TargetType), str);
        }

        #endregion

        /// <summary>
        /// Checks if a Component that can be animated with the given animationType is attached to the src
        /// </summary>
        bool Validate(GameObject _target)
        {
            if (animationType == DOTweenAnimationType.None) return false;

            Component srcTarget;
            // First check for external plugins
#if DOTWEEN_TK2D
            if (_Tk2dAnimationTypeToComponent.ContainsKey(_src.animationType)) {
                foreach (Type t in _Tk2dAnimationTypeToComponent[_src.animationType]) {
                    srcTarget = _src.GetComponent(t);
                    if (srcTarget != null) {
                        _src.target = srcTarget;
                        _src.targetType = DOTweenAnimation.TypeToDOTargetType(t);
                        return true;
                    }
                }
            }
#endif
#if DOTWEEN_TMP
            if (_TMPAnimationTypeToComponent.ContainsKey(_src.animationType)) {
                foreach (Type t in _TMPAnimationTypeToComponent[_src.animationType]) {
                    srcTarget = _src.GetComponent(t);
                    if (srcTarget != null) {
                        _src.target = srcTarget;
                        _src.targetType = DOTweenAnimation.TypeToDOTargetType(t);
                        return true;
                    }
                }
            }
#endif
            // Then check for regular stuff
            if (_AnimationTypeToComponent.ContainsKey(animationType))
            {
                foreach (Type t in _AnimationTypeToComponent[animationType])
                {
                    srcTarget = _target.GetComponent(t);
                    if (srcTarget != null)
                    {
                        targetComponent = srcTarget;
                        return true;
                    }
                }
            }
            return false;
        }
        #region Implemention of ICloneable
        public object Clone()
        {
            return new DtAnim()
            {
                animationType = animationType,
                delay = delay,
                duration = duration,
                easeCurve = easeCurve,
                easeType = easeType,
                endValueColor = endValueColor,
                endValueFloat = endValueFloat,
                endValueRect = endValueRect,
                endValueString = endValueString,
                endValueV2 = endValueV2,
                endValueV3 = endValueV3,
                id = id,
                isFrom = isFrom,
                isIndependentUpdate = isIndependentUpdate,
                isRelative = isRelative,
                isSpeedBased = isSpeedBased,
                loops = loops,
                loopType = loopType,
                optionalBool0 = optionalBool0,
                optionalFloat0 = optionalFloat0,
                optionalInt0 = optionalInt0,
                optionalRotationMode = optionalRotationMode,
                optionalScrambleMode = optionalScrambleMode,
                optionalString = optionalString,
            };
        }
        #endregion
    }
}