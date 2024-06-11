using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    public void AddPlayer(Transform transform){
        playersTransform.Add(transform);
    }

    public bool HasPlayer(Transform transform){
        if (playersTransform.Contains(transform)){
            return false;
        }
        return true;
    }
}
