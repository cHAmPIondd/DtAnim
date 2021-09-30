using DG.Tweening;
using DtAnim;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public DtAnimBehavior show;
    public DtAnimBehavior hide;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            show.DOPlay();
    }
}
