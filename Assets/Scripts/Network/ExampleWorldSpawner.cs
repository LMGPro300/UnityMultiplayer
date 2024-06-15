using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Example script to spawn something in every 1 second
/*
 * Program name: ExampleWorldSpawner.cs
 * Author: Elvin Shen 
 * What the program does: Testing my SycnWithWorldSpace script to spawn network object into the network
 */
public class ExampleWorldSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab; //prefab MUST HAVE NETWORK OBJECT COMPONENT
    CountdownTimer spawnTimer = new CountdownTimer(10f);


    public void Start(){
        spawnTimer.Start();
        spawnTimer.OnTimerStop += () => {
            spawnTimer.Start();
            SpawnIn();
        };
    }

    public void Update(){
        spawnTimer.Tick(Time.deltaTime);
    }


    public void SpawnIn(){
        SyncWithWorldSpace.Instance.InstantiateOnServer(prefab, transform.position, new Quaternion(0f,0f,0f,1)); //spawn the prefab
    }

}
