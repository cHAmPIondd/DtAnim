using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DtAnim
{
    [System.Serializable]
    public class DtAnimBehavior
    {
        public event System.Action onPlay;
        public event System.Action onStop;
        public event System.Action onComplete;

        [SerializeField]
        internal DtAnim dtAnim = new DtAnim();
        [SerializeField]
        internal GameObject target;
        [SerializeField]
        internal bool isLoadPresetAtRuntime;
        [SerializeField]
        internal string categoryName;
        [SerializeField]
        internal string presetName;
        [SerializeField]
        internal bool isFoldOut;

        internal Tween m_tween;
        internal bool IsTweenCreated { get { return m_tween != null; } }
        /// <summary>
        /// When equal to true, the onComplete callback is not triggered
        /// </summary>
        private bool isDisableCompleteCallback;
        ~DtAnimBehavior()
        {
            Reset();
        }
        /// <summary>
        /// Reset status to before playback
        /// </summary>
        internal void Reset()
        {
            if (m_tween != null)
            {
                if (dtAnim.isFrom)
                {
                    isDisableCompleteCallback = true;
                    m_tween.Complete();
                    isDisableCompleteCallback = false;
                }
                else
                {
                    m_tween.Rewind();
                    onStop?.Invoke();
                }
                m_tween.Kill();
                m_tween = null;
            }
        }
        internal void CreateNewTween()
        {
            Assert.IsNull(m_tween);
            Assert.IsNotNull(target);
            if (isLoadPresetAtRuntime)
                dtAnim = DtAnimPresetManager.Instance.LoadPreset(categoryName, presetName);
            m_tween = dtAnim.CreateNewTween(target);
            if (m_tween != null)
            {
                m_tween.onComplete += () =>
                {
                    if (isDisableCompleteCallback == false)
                        onComplete?.Invoke();
                    onStop?.Invoke();
                };
            }
        }
        /// <summary>
        /// Replay the tween, create a new tween when the tween is null
        /// </summary>
        /// <param name="_onComplete"></param>
        public void DOPlay()
        {
            if (m_tween == null)
            {
                CreateNewTween();
            }
            if (m_tween != null)
            {
                onPlay?.Invoke();
                m_tween.Restart();
            }
        }
        /// <summary>
        /// Complete playing tween
        /// </summary>
        /// <param name="_withCallbacks">Determines whether the onComplete callback is triggered</param>
        public void DOComplete(bool _withCallbacks)
        {
            if (m_tween != null && m_tween.IsPlaying())
            {
                isDisableCompleteCallback = _withCallbacks == false;
                m_tween.Complete();
                isDisableCompleteCallback = false;
            }
        }
        /// <summary>
        /// Stop tween in the current frame state
        /// </summary>
        public void DOKill()
        {
            if (m_tween != null && m_tween.IsPlaying())
            {
                onStop?.Invoke();
                m_tween.Kill();
                m_tween = null;
            }
        }
    }
}