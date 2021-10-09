using DG.Tweening;
using DtAnim;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public DtAnimBehavior show;
    public DtAnimBehavior hide;
    private void Awake()
    {
        show.onComplete += () => {
            Debug.Log("Complete");
        };
        show.onPlay += () => {
            Debug.Log("Play");
        };
        show.onStop += () => {
            Debug.Log("Stop");
        };
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            show.DOPlay();
        if(Input.GetKeyDown(KeyCode.F2))
        {
            show.DOComplete(true);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            show.DOComplete(false);
        }
    }
}
