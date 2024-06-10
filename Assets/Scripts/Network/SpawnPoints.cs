using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;   
    private int numSpawnPoints;
    private Vector3[] positionSpawnPoints;
    public static SpawnPoints Instance { get; private set; }

    public void Awake(){
        Instance = this;
        numSpawnPoints = spawnPoints.Length;
        positionSpawnPoints = new Vector3[numSpawnPoints];
        for (int i = 0; i < numSpawnPoints; i++){
            positionSpawnPoints[i] = spawnPoints[i].position;
        }
    }

    public Vector3 RandomSpawnPoint(){
        return positionSpawnPoints[Random.Range(0, numSpawnPoints)];
    }  
}
