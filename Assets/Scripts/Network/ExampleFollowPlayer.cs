using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ExampleFollowPlayer : NetworkBehaviour
{
    [SerializeField] GameObject prefab; //prefab to follow parent
    [SerializeField] FollowNetworkTransform followNetworkTransform; //just drag the prefab to this location
    [SerializeField] Transform followObject; //gameobject you want to follow (you might need to use the gameobject finder)
    CountdownTimer spawnTimer = new CountdownTimer(5f);
    CountdownTimer destroyTimer = new CountdownTimer(10f);
    [SerializeField] Transform final;

    GameObject last;

    void Start(){
        if (!IsOwner) return;
        spawnTimer.Start();
        destroyTimer.Start();
        spawnTimer.OnTimerStop += () => {SpawnIn();};
        destroyTimer.OnTimerStop += () => {DestroyOut();};
    }

    public void Update(){
        spawnTimer.Tick(Time.deltaTime);
        destroyTimer.Tick(Time.deltaTime);
    }

    void DestroyOut(){
        SyncWithWorldSpace.Instance.DestoryOnServer(last);
    }

    void SpawnIn(){
        SyncWithWorldSpace.Instance.InstantiateOnServer(prefab, new Vector3(0,0,0), new Quaternion(0f,0f,0f,1));
        last = SyncWithWorldSpace.Instance.lastSpawnedGameObject;
        followNetworkTransform = SyncWithWorldSpace.Instance.lastSpawnedGameObject.GetComponent<FollowNetworkTransform>();
        //followNetworkTransform.SetTargetTransform(final);
        SetTargetTransformRequest(followObject);
    }

    public void SetTargetTransformRequest(Transform targetTransform){
        SetTargetTransformServerRpc(targetTransform.GetComponent<NetworkObject>());
    }

    public void SetTargetTransformRequest(NetworkObject targetObject){
        SetTargetTransformServerRpc(targetObject);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SetTargetTransformServerRpc(NetworkObjectReference transformParent){
        SetTargetTransformClientRpc(transformParent);
    }

    [ClientRpc]
    private void SetTargetTransformClientRpc(NetworkObjectReference transformParent){
        transformParent.TryGet(out NetworkObject targetObject);
        Relocator relocator = targetObject.GetComponent<Relocator>();
        GameObject other = relocator.GetOtherLocation();
        //this.final = other.transform;
        followNetworkTransform.SetTargetTransform(other.transform);
    }
}