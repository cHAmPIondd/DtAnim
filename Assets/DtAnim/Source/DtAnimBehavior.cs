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
        internal bool IsPlaying { get; set; }
        ~DtAnimBehavior()
        {
            ClearTween();
        }
        internal void ClearTween()
        {
            DORewind();
            m_tween.Kill();
            m_tween = null;
        }
        public void DOPlay()
        {
            Assert.IsNotNull(target);
            Assert.IsNull(m_tween);
            if (m_tween == null)
            {
                if (isLoadPresetAtRuntime)
                    dtAnim = DtAnimPresetManager.Instance.LoadPreset(categoryName, presetName);
                m_tween = dtAnim.CreateNewTween(target);
            }
            m_tween?.Restart();
            IsPlaying = true;
        }
        public void DORewind()
        {
            dtAnim.Rewind(m_tween);
            IsPlaying = false;
        }
    }
}