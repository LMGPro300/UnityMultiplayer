using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Program name: NetworkTimer.cs
 * Author: Elvin Shen 
 * What the program does: Tick counter for the server
 */

public class NetworkTimer
{
    private float timer;
    private float MinTimeBetweenTick { get; }
    public int currentTick { get; private set; }


    public NetworkTimer(float serverTickRate){
        MinTimeBetweenTick = 1f / serverTickRate;
    }

    public void Update(float deltaTime){ //make "time" past does, not equal a tick
        timer += deltaTime;
    }

    public float networkDeltaTime(float deltaTime){
        //Debug.Log(Time.deltaTime);
        //Debug.Log(deltaTime);
        return MinTimeBetweenTick;//Time.deltaTime;//MinTimeBetweenTick;// Time.fixedDeltaTime;//(MinTimeBetweenTick / (1f / Time.deltaTime)) * (1f/MinTimeBetweenTick)/2;//MinTimeBetweenTick * (MinTimeBetweenTick / (1f / Time.deltaTime));//MinTimeBetweenTick / (60f / Time.deltaTime);
    }

    public bool ShouldTick(){
        if (timer > MinTimeBetweenTick){ //if we should tick
            timer -= MinTimeBetweenTick; //tick but also keep the time over the min tick time
            currentTick ++;
            return true;
        }
        return false;
    }
}
    
