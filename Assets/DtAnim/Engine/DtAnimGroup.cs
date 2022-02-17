using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace DtAnim
{
    [System.Serializable]
    public class DtAnimGroup
    {
        public event Action onComplete;
        [SerializeField]
        public List<DtAnim> dtAnims;
        public bool HasAnim { get => dtAnims.Count > 0; }
        public bool IsTweenCreated { get => m_curTweens.Count > 0; }
        public bool IsPlaying { get => m_playingCount > 0; }
        private Dictionary<DtAnim, DtAnimTween> m_curTweens = new Dictionary<DtAnim, DtAnimTween>();
        private int m_playingCount;
        public void Reset()
        {
            foreach (var tween in m_curTweens.Values)
            {
                tween.Reset();
            }
            m_curTweens.Clear();
            m_playingCount = 0;
        }
        public void CreateTweens(GameObject _target)
        {
            Assert.IsFalse(IsTweenCreated);
            foreach (var anim in dtAnims)
            {
                var newTween = anim.CreateNewTween(_target);
                if (newTween != null)
                    m_curTweens.Add(anim, newTween);
            }
        }
        public void Play(Action _tempOnComplete = null)
        {
            if (m_curTweens.Count > 0)
            {
                m_playingCount = m_curTweens.Count;
                foreach (var tween in m_curTweens.Values)
                {
                    tween.Play(() =>
                    {
                        m_playingCount--;
                        if (m_playingCount <= 0)
                        {
                            _tempOnComplete?.Invoke();
                            onComplete?.Invoke();
                        }
                    });
                }
            }
        }
        public void Complete(bool _withCallbacks)
        {
            foreach (var tween in m_curTweens.Values)
            {
                tween.Complete(_withCallbacks);
            }
            m_playingCount = 0;
        }
        public DtAnimGroup Clone()
        {
            return new DtAnimGroup()
            {
                dtAnims = dtAnims.Select(x => x.Clone()).ToList()
            };
        }
    }
    public class DtAnimTween
    {
        private Tween m_tween;
        private bool m_isFromValue;
        private System.Action m_completeHandler;
        public DtAnimTween(Tween _tween, bool _isFromValue)
        {
            m_tween = _tween;
            m_isFromValue = _isFromValue;
            m_tween.onComplete += () =>
            {
                m_completeHandler?.Invoke();
            };
        }
        public void Reset()
        {
            m_completeHandler = null;
            if (m_isFromValue)
            {
                if (m_tween.Loops() == -1)
                {
                    m_tween.SetLoops(1);
                    m_tween.Complete();
                    m_tween.SetLoops(-1);
                }
                else
                {
                    m_tween.Complete();
                }
            }
            else
            {
                m_tween.Rewind();
            }
            m_tween.Kill();
            m_tween = null;
        }
        public void Play(System.Action _completeHandler)
        {
            if (Application.isPlaying)
            {
                m_completeHandler = _completeHandler;
                m_tween.Restart();
            }
            else
            {
#if UNITY_EDITOR
                DG.DOTweenEditor.DOTweenEditorPreview.PrepareTweenForPreview(m_tween);
                DG.DOTweenEditor.DOTweenEditorPreview.Start(() =>
                {
                    if (m_tween.IsPlaying() == false)
                    {
                        _completeHandler?.Invoke();
                        DG.DOTweenEditor.DOTweenEditorPreview.Stop();
                    }
                });
#endif
            }
        }
        public void Complete(bool _withCallbacks)
        {
            m_tween.Complete(_withCallbacks);
        }
    }
}