using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Program name: SpawnPoints.cs
 * Author: Elvin Shen 
 * What the program does: Handles random spawns for the players to spawn in
 */
public class SpawnPoints : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;   
    private int numSpawnPoints;
    private Vector3[] positionSpawnPoints;
    public static SpawnPoints Instance { get; private set; }

    //get all the spawn points into Vector3
    public void Awake(){
        Instance = this;
        numSpawnPoints = spawnPoints.Length;
        positionSpawnPoints = new Vector3[numSpawnPoints];
        for (int i = 0; i < numSpawnPoints; i++){
            positionSpawnPoints[i] = spawnPoints[i].position;
        }
    }
    
    //generate a random spawn point
    public Vector3 RandomSpawnPoint(){
        return positionSpawnPoints[Random.Range(0, numSpawnPoints)];
    }  
}
