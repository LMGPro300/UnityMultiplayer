using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameLogic : NetworkBehaviour{
    private CountdownTimer daytimeTimer;

    [SerializeField] private Transform[] nightPos;
    private Vector3[] nightPositions;
    private int numNightPositions;

    [SerializeField] private Transform[] dayPos;
    private Vector3[] dayPositions;
    private int numDayPositions;

    [SerializeField] private Transform[] spawnPos;
    private Vector3[] spawnPositions;
    private int numSpawnPositions;

    private int[] waveData = {5, 15, 50};
    private int wave = -1;

    private GameObject zombie;

    private void Awake(){
        if (!IsServer) return;
        daytimeTimer = new CountdownTimer(300);
        daytimeTimer.OnTimerStop += () => {TriggerNight();};

        numNightPositions = nightPos.Length;
        nightPositions = new Vector3[numNightPositions];
        for(int i = 0; i < numNightPositions; i++){
            nightPositions[i] = nightPos[i].position;
        }

        numDayPositions = dayPos.Length;
        dayPositions = new Vector3[numDayPositions];
        for(int i = 0; i < numDayPositions; i++){
            dayPositions[i] = dayPos[i].position;
        }

        numSpawnPositions = spawnPos.Length;
        spawnPositions = new Vector3[numSpawnPositions];
        for(int i = 0; i < numSpawnPositions; i++){
            spawnPositions[i] = spawnPos[i].position;
        }
    }

    private void Update(){
        if (!IsServer) return;
        daytimeTimer.Tick(Time.deltaTime);
    }

    private void TriggerNight(){
        //make things dark
        wave += 1;
        for (int i = 0; i < waveData[wave]; i++){

        }
    }
    
}
