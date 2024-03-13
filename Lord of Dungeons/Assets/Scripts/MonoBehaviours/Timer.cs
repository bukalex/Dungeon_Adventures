using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Timer : MonoBehaviour
{
    public float timer;
    private Action timerCallback;

    protected virtual void SetTimer(float timer, Action timerCallback)
    {
        this.timer = timer;
        this.timerCallback = timerCallback;
    }
    // Update is called once per frame
    protected virtual void TimerUpdate()
    {
        if(timer > 0f)
        {
            timer -= Time.deltaTime;
            if (IsTimerComplete()){
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
