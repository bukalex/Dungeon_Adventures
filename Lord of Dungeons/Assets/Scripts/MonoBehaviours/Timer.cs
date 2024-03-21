using System;
using UnityEngine;

public abstract class Timer : MonoBehaviour
{
    public float timer;
    private Action timerCallback;

    protected void SetTimer(float timer, Action timerCallback)
    {
        this.timer = timer;
        this.timerCallback = timerCallback;
    }
    // Update is called once per frame
    protected void TimerUpdate()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (IsTimerComplete())
            {
                Debug.Log("Time is completed");
                timerCallback();
            }
        }
    }

    private bool IsTimerComplete()
    {
        return timer <= 0f;
    }
}
