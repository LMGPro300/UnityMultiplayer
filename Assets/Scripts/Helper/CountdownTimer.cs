using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;

/*
 * Program name: CountdownTimer.cs
 * Author: Elvin Shen
 * What the program does: A timer that counts in seconds
 */


public class CountdownTimer
{
    private float seconds;
    private bool finished;
    private float timeLeft;

    public Action OnTimerStop = delegate { };
    public CountdownTimer(float seconds){
        this.seconds = seconds;
        this.finished = true;
        this.timeLeft = 0f;
    }
    //set the time left to the targettime
    public void Start(){
        timeLeft = seconds;
        this.finished = false;
    }

    public void Stop()
    {
        this.finished = true;
    }

    //subtract time from the time left
    public void Tick(float deltaTime){
        if (finished) return;
        if (timeLeft > 0){
            timeLeft -= deltaTime;
        } else{
            finished = true;
            OnTimerStop.Invoke();
        }
    }

    public bool IsRunning(){
        return !finished;
    }

    public bool IsFinished(){
        return finished;
    }
    
    public void SetNewTime(float newTime)
    {
        this.seconds = newTime;
        this.finished = true;
        this.timeLeft = 0f;
    }
}
