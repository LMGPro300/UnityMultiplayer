using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameLogic : NetworkBehaviour{
    [SerializeField] private SpawnableScriptableObject spawnableScriptableObject;
    [SerializeField] private int itemSpawnChance;
    [SerializeField] private int daySpawnChance;
    [SerializeField] private TextMeshProUGUI notifyMsg;
    [SerializeField] private Light lighting;

    [SerializeField] private Transform[] nightPos;
    private Vector3[] nightPositions;
    private int numNightPositions;

    [SerializeField] private Transform[] dayPos;
    private Vector3[] dayPositions;
    private int numDayPositions;

    [SerializeField] private Transform[] spawnPos;
    private Vector3[] spawnPositions;
    private int numSpawnPositions;

    private int[] nightWaveData = {5, 15, 25, 40, 75};
    private int[] dayWaveData = {5, 10, 15, 20, 25};
    private int wave = 0;

    [SerializeField] private GameObject zombiePrefab;
    private int enemyLeft = 0;

    private CountdownTimer daytimeTimer;
    private CountdownTimer warningTimer = new CountdownTimer(10);
    private CountdownTimer inBetweenTimer = new CountdownTimer(10);
    private CountdownTimer clearTextTimer = new CountdownTimer(5);
    private List<GameObject> remainingDayEnemies;

    private bool IsNight = false;
    

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        if (!IsServer) return;
        spawnableScriptableObject.Init();
        daytimeTimer = new CountdownTimer(120);
        daytimeTimer.OnTimerStop += () => {NightWarning();};
        daytimeTimer.Start();
        warningTimer.OnTimerStop += () => {TriggerNight();};
        remainingDayEnemies = new List<GameObject>();

        inBetweenTimer.OnTimerStop += () => {DayStart();};
        inBetweenTimer.Start();

        clearTextTimer.OnTimerStop += () => {notifyMsg.text = "";};

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
        warningTimer.Tick(Time.deltaTime);
        inBetweenTimer.Tick(Time.deltaTime);
        clearTextTimer.Tick(Time.deltaTime);

        if (IsNight && lighting.intensity > 0){
            lighting.intensity -= 0.001f;
        } else if (!IsNight && lighting.intensity < 1){
            lighting.intensity += 0.001f;
        }

    }

    private void NightWarning(){
        notifyMsg.text = "They will be here soon...";
        warningTimer.Start();
        clearTextTimer.Start();
        IsNight = true;

    }

    private void TriggerNight(){
        //make things dark
        Debug.Log("the night has started");
        notifyMsg.text = "The wave is coming...";
        clearTextTimer.Start();
        wave += 1;
        enemyLeft = nightWaveData[wave];
        for (int i = 0; i < enemyLeft; i++){
            Vector3 randPos = nightPositions[Random.Range(0, numNightPositions-1)];
            SyncWithWorldSpace.Instance.InstantiateOnServer(zombiePrefab, randPos, Quaternion.identity);
            GameObject lastEnemy = SyncWithWorldSpace.Instance.lastSpawnedGameObject;
            Entity enemyEntity = lastEnemy.GetComponent<Entity>();
            enemyEntity.OnDeath += () => {enemyLeft -= 1; IsNightOver();};
        }

        foreach(GameObject remain in remainingDayEnemies){
            SyncWithWorldSpace.Instance.DestoryOnServer(remain);
        }
    }

    private void DayStart(){
        notifyMsg.text = "Day " + (this.wave+1) + ", don't die out there";
        clearTextTimer.Start();
    }

    private void IsNightOver(){
        if (enemyLeft != 0) return;
        
        daytimeTimer.Start();
        notifyMsg.text = "The night is coming to an end...";
        clearTextTimer.Start();
        IsNight = false;
        Debug.Log("the day is coming back!!!");
        

        for (int i = 0; i < numSpawnPositions; i ++){
            int chance = Random.Range(0, 100);
            if (chance > itemSpawnChance) continue;
            Debug.Log("spawned for " + i);

            int itemChance = Random.Range(0, 100);
            int itemType = 0;
            int cumulChance = 0;
            for (int j = 0; j < 4; j++){
                cumulChance += spawnableScriptableObject.totalChances[j];
                if (itemChance < cumulChance){
                    itemType = j;
                    break;
                }
            }

            GameObject targetSpawnObject;

            if (itemType == 0){
                targetSpawnObject = spawnableScriptableObject.RandomGun();
            } else if (itemType == 1){
                targetSpawnObject = spawnableScriptableObject.RandomMelee();
            } else if (itemType == 2){
                targetSpawnObject = spawnableScriptableObject.RandomItem();
            } else {
                targetSpawnObject = spawnableScriptableObject.RandomGarbage();
            }
            SyncWithWorldSpace.Instance.InstantiateOnServer(targetSpawnObject, spawnPositions[i], Quaternion.identity);
        }


        for (int i = 0; i < dayWaveData[wave]; i++){
            int chance = Random.Range(0, 100);
            if (chance > daySpawnChance) continue;
            SyncWithWorldSpace.Instance.InstantiateOnServer(zombiePrefab, dayPositions[Random.Range(0, numDayPositions)], Quaternion.identity);
            remainingDayEnemies.Add(SyncWithWorldSpace.Instance.lastSpawnedGameObject);
        }

        inBetweenTimer.Start();
    }
}
