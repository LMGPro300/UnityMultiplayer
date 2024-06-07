using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class ItemSync : NetworkBehaviour{
    private GameObject targetObject;
    private int targetIndex = -1;
    //[SerializeField] FollowNetworkTransform followNetworkTransform;
    [SerializeField] private GlobalPrefabNetworkList globalPrefabNetworkList;
    private List<GameObject> lastSpawnedItem = new List<GameObject>();
    [SerializeField] private NetworkObject playerObject;
    private List<GameObject> itemsToRemove = new List<GameObject>();
    private int clientId;//NetworkManager.Singleton.LocalClientId;
    private int globalId;

    public override void OnNetworkSpawn(){
        globalId += 1;
        base.OnNetworkSpawn();
        if (!IsOwner) return; 
        clientId = globalId;
    }

    public int PrefabToIndex(GameObject prefab){
        return globalPrefabNetworkList._globalPrefabNetworkList.IndexOf(prefab);
    }
    
    public GameObject IndexToPrefab(int index){
        return globalPrefabNetworkList._globalPrefabNetworkList[index];
    }

    public void GetNewObject(GameObject prefab){
        Clear();
        targetIndex = PrefabToIndex(prefab);
        SpawnObjectGlobal();
    }

    public void Clear(){
        targetIndex = -1;
        
        foreach (GameObject GO in lastSpawnedItem){
            if (GO == null) continue;
            SyncWithWorldSpace.Instance.DestoryOnServer(GO);
            itemsToRemove.Add(GO);
        }

        foreach (GameObject remove in itemsToRemove){
            lastSpawnedItem.Remove(remove);
        }
        itemsToRemove.Clear();
    }

    public void SpawnObjectGlobal(){
        Debug.Log(clientId + " has this id");
        if (targetIndex == -1) return;
        if (IsServer) { SpawnObjectLocal(targetIndex, clientId); }
        else { RequestSpawnGlobalServerRpc(targetIndex, clientId); }
    }


    private void SpawnObjectLocal(int index, int senderId){
        GameObject prefab = IndexToPrefab(index);
        GameObject spawnedObject = Instantiate(prefab);  
        NetworkObject instanceNetworkObject = spawnedObject.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
        ParentToCharClientRpc(playerObject, instanceNetworkObject, senderId);
        //lastSpawnedItem.Add(spawnedObject);
    }

    [ServerRpc]
    private void RequestSpawnGlobalServerRpc(int index, int senderId){
        SpawnObjectLocal(index, senderId);
    }

    [ClientRpc] 
    private void ParentToCharClientRpc(NetworkObjectReference playerObject, NetworkObjectReference itemObject, int senderId){
        playerObject.TryGet(out NetworkObject targetPlayerObject);
        itemObject.TryGet(out NetworkObject targetItemObject);
        Relocator relocator = targetPlayerObject.GetComponent<Relocator>();
        GameObject other = relocator.GetOtherLocation();
        FollowNetworkTransform followNetworkTransform = targetItemObject.GetComponent<FollowNetworkTransform>();
        if (senderId == clientId){
            targetItemObject.GetComponent<MeshRenderer>().enabled = false;
        }
        followNetworkTransform.SetTargetTransform(other.transform);
        lastSpawnedItem.Add(targetItemObject.transform.gameObject);
    }
}
