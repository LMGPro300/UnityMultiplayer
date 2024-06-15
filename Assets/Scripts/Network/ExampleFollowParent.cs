using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/*
 * Program name: ExampleFollowParent.cs
 * Author: Elvin Shen 
 * What the program does: Testing my SycnWithWorldSpace script to support following a parent
 *                          Not really used in-game
 */

public class ExampleFollowParent : MonoBehaviour
{
    [SerializeField] GameObject prefab; //prefab to follow parent
    [SerializeField] FollowNetworkTransform followNetworkTransform; //just drag the prefab to this location
    [SerializeField] Transform followObject; //gameobject you want to follow (you might need to use the gameobject finder)
    CountdownTimer spawnTimer = new CountdownTimer(5f);


    void Start(){
        spawnTimer.Start();
        spawnTimer.OnTimerStop += () => {SpawnIn();};
    }

    public void Update(){
        spawnTimer.Tick(Time.deltaTime);
    }
    
    void SpawnIn(){
        SyncWithWorldSpace.Instance.InstantiateOnServer(prefab, new Vector3(0,0,0), new Quaternion(0f,0f,0f,1));
        SetTargetTransformRequest(followObject);
    }

    public void SetTargetTransformRequest(Transform targetTransform){
        SetTargetTransformServerRpc(targetTransform.GetComponent<NetworkObject>());
    }

    public void SetTargetTransformRequest(NetworkObject targetObject){
        SetTargetTransformServerRpc(targetObject);
    }


    [ServerRpc]
    private void SetTargetTransformServerRpc(NetworkObjectReference transformParent){
        SetTargetTransformClientRpc(transformParent);
    }

    [ClientRpc]
    private void SetTargetTransformClientRpc(NetworkObjectReference transformParent){
        transformParent.TryGet(out NetworkObject targetObject);
        followNetworkTransform.SetTargetTransform(targetObject.transform);
    }
}
