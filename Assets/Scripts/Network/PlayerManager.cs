using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Program name: PlayerManager.cs
 * Author: Elvin Shen 
 * What the program does: Stores all the players information when spawned in
 */

public class PlayerManager : MonoBehaviour{
    public static PlayerManager Instance { get; private set; }
    public List<Transform> playersTransform;

    void Awake(){
        Instance = this;
        playersTransform = new List<Transform>();
    }

    private void Update()
    {
        Debug.Log(playersTransform.Count + "NOAH");
    }

    //if the player doesnt exist yet, add it in
    public void AddPlayer(Transform transform){
        if (HasPlayer(transform)){
            playersTransform.Add(transform);
            Debug.Log(playersTransform);
        }
    }
    //check if the player already exists
    public bool HasPlayer(Transform transform){
        if (playersTransform.Contains(transform)){
            return false;
        }
        return true;
    }
}
