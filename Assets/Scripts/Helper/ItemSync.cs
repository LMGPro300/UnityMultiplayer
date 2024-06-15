using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/*
 * Program name: ItemSync.cs
 * Author: Elvin Shen 
 * What the program does: Syncs item locations and information in the netowork
 */

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

    //assign a unique client id
    public override void OnNetworkSpawn(){
        globalId += 1;
        base.OnNetworkSpawn();
        if (!IsOwner) return; 
        clientId = globalId;
    }

    //Unable to send prefab through rpcs, resorted to mapping indexes to prefabs
    public int PrefabToIndex(GameObject prefab){
        return globalPrefabNetworkList._globalPrefabNetworkList.IndexOf(prefab);
    }
    
    //turn index to the prefab
    public GameObject IndexToPrefab(int index){
        return globalPrefabNetworkList._globalPrefabNetworkList[index];
    }

    //spawn a new object by prefab
    public void GetNewObject(GameObject prefab){
        Clear();
        targetIndex = PrefabToIndex(prefab);
        SpawnObjectGlobal();
    }

    //clear the last spawned items list
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

    //Spawn a new object into the network
    public void SpawnObjectGlobal(){
        Debug.Log(clientId + " has this id");
        if (targetIndex == -1) return;
        //the clients are unable to spawn items, and can only request to spawn items
        if (IsServer) { SpawnObjectLocal(targetIndex, clientId); }
        else { RequestSpawnGlobalServerRpc(targetIndex, clientId); }
    }

    //Spawn an object onto the server (even from clients)
    private void SpawnObjectLocal(int index, int senderId){
        GameObject prefab = IndexToPrefab(index);
        GameObject spawnedObject = Instantiate(prefab);  
        NetworkObject instanceNetworkObject = spawnedObject.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
        ParentToCharClientRpc(playerObject, instanceNetworkObject, senderId);
        //lastSpawnedItem.Add(spawnedObject);
    }

    //Request to spawn an object
    [ServerRpc]
    private void RequestSpawnGlobalServerRpc(int index, int senderId){
        SpawnObjectLocal(index, senderId);
    }

    //Since spawning under a parent is not possible in netcode, the only way
    //possible is to have the spawned item follow an existing object already parented
    [ClientRpc] 
    private void ParentToCharClientRpc(NetworkObjectReference playerObject, NetworkObjectReference itemObject, int senderId){
        playerObject.TryGet(out NetworkObject targetPlayerObject);
        itemObject.TryGet(out NetworkObject targetItemObject);
        Relocator relocator = targetPlayerObject.GetComponent<Relocator>();
        GameObject other = relocator.GetOtherLocation();
        FollowNetworkTransform followNetworkTransform = targetItemObject.GetComponent<FollowNetworkTransform>();
        //if we were the sender, hide the object
        if (senderId == clientId){
            targetItemObject.transform.gameObject.SetActive(false);
        }
        followNetworkTransform.SetTargetTransform(other.transform);
        lastSpawnedItem.Add(targetItemObject.transform.gameObject);
    }
}
