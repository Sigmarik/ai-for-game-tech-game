using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Image progressImage;
    private float startTime;
    private float endTime;
    private bool isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        progressImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            progressImage.fillAmount = (Time.time - startTime) / (endTime - startTime);
        }
    }

    public void StartProgressBar(float time)
    {
        startTime = Time.time;
        endTime = Time.time + time;
        isRunning = true;
    }

    public void StopProgressBar()
    {
        progressImage.fillAmount = 0;
        isRunning = false;
    }
}
