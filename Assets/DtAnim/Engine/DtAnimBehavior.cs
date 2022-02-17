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
        public DtAnimGroup dtAnimGroup = new DtAnimGroup();
        [SerializeField]
        public GameObject target;
        [SerializeField]
        public bool isLoadPresetAtRuntime;
        [SerializeField]
        public string categoryName;
        [SerializeField]
        public string presetName;
        [SerializeField]
        public bool isFoldOut;
        public bool IsTweenCreated { get { return dtAnimGroup.IsTweenCreated; } }

        /// <summary>
        /// Reset status to before playback
        /// </summary>
        public void Reset()
        {
            dtAnimGroup.Reset();
        }
        public void CreateTweens()
        {
            Assert.IsFalse(IsTweenCreated);
            if (target == null) return;
            if (isLoadPresetAtRuntime)
                dtAnimGroup = DtAnimPresetManager.Instance.LoadPreset(categoryName, presetName);
            dtAnimGroup.CreateTweens(target);
        }
        /// <summary>
        /// Replay the tween, create a new tween when the tween is null
        /// </summary>
        /// <param name="_onComplete"></param>
        public void DOPlay(System.Action _tempOnComplete = null)
        {
            if (dtAnimGroup.IsTweenCreated == false)
            {
                CreateTweens();
            }
            dtAnimGroup.Play(_tempOnComplete);
        }
        /// <summary>
        /// Complete playing tween
        /// </summary>
        /// <param name="_withCallbacks">Determines whether the onComplete callback is triggered</param>
        public void DOComplete(bool _withCallbacks)
        {
            if (dtAnimGroup.IsPlaying)
            {
                dtAnimGroup.Complete(_withCallbacks);
            }
        }
    }
}