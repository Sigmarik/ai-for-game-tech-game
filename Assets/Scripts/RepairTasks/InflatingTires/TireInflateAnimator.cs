using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TireInflateAnimator : MonoBehaviour
{
    public Vector3 deflatedScale = new Vector3(0.6f, 0.6f, 0.6f);
    public Vector3 inflatedScale = new Vector3(1.0f, 1.0f, 1.0f);
    public float duration = 1.2f;

    void Start()
    {
        Play();
    }
    public void Play()
    {
        transform.DOScale(inflatedScale, duration);
    }

    public void Kill()
    {
        
    }
}
