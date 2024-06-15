using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameLogic : NetworkBehaviour{
    [SerializeField] private SpawnableScriptableObject spawnableScriptableObject;
    [SerializeField] private int itemSpawnChance;
    [SerializeField] private int daySpawnChance;

    [SerializeField] private Transform[] nightPos;
    private Vector3[] nightPositions;
    private int numNightPositions;

    [SerializeField] private Transform[] dayPos;
    private Vector3[] dayPositions;
    private int numDayPositions;

    [SerializeField] private Transform[] spawnPos;
    private Vector3[] spawnPositions;
    private int numSpawnPositions;

    private int[] nightWaveData = {1,1,1};//{5, 15, 50};
    private int[] dayWaveData = {1,1,1};
    private int wave = -1;

    [SerializeField] private GameObject zombiePrefab;
    private int enemyLeft;

    private CountdownTimer daytimeTimer;
    private List<GameObject> remainingDayEnemies;

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        if (!IsServer) return;
        spawnableScriptableObject.Init();
        daytimeTimer = new CountdownTimer(10);
        daytimeTimer.OnTimerStop += () => {TriggerNight();};
        daytimeTimer.Start();
        remainingDayEnemies = new List<GameObject>();

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
        Debug.Log("the night has started");
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

    private void IsNightOver(){
        if (enemyLeft != 0) return;
        
        daytimeTimer.Start();
        Debug.Log("the day is coming back!!!");
        

        for (int i = 0; i < numSpawnPositions; i ++){
            int chance = Random.Range(0, 100);
            if (chance > itemSpawnChance) continue;

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
    }
}
