using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;

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
    public void Start(){
        timeLeft = seconds;
        this.finished = false;
    }

    public void Stop()
    {
        this.finished = true;
    }

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
    
}
