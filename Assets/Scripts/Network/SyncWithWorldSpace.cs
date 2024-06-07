using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class SyncWithWorldSpace: NetworkBehaviour
{
    public static SyncWithWorldSpace Instance { get; private set; }

    [SerializeField] private PrefabNetworkList prefabNetworkList;
    public GameObject lastSpawnedGameObject;

    private void Awake(){
        Instance = this;
    }

    public int PrefabToIndex(GameObject prefab){
        return prefabNetworkList._prefabNetworkList.IndexOf(prefab);
    }
    
    public GameObject IndexToPrefab(int index){
        return prefabNetworkList._prefabNetworkList[index];
    }
    
    public void DestoryOnServer(GameObject obj){
        if (obj == null) return;
        NetworkObject nObj = obj.GetComponent<NetworkObject>();
        if (nObj == null || !nObj.IsSpawned) return;
        DestoryOnServerServerRpc(nObj);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestoryOnServerServerRpc(NetworkObjectReference networkObjectReference){
        networkObjectReference.TryGet(out NetworkObject obj);
        if (obj == null) return;
        Debug.Log("GOT GONED LLL");
        obj.Despawn(true);
    }

    public void InstantiateOnServer(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 force){
        int index = PrefabToIndex(prefab);
        //GameObject spawnedObject = Instantiate(prefab);
        //NetworkObject spawnedNetwork = spawnedObject.GetComponent<NetworkObject>();
        //spawnedNetwork.Spawn();
        if (IsServer) { SpawnByServer(index, position, rotation, force); }
        else { SpawnServerRpc(index, position, rotation, force); }
    }

    public void InstantiateOnServer(GameObject prefab, Vector3 position, Quaternion rotation){
        int index = PrefabToIndex(prefab);
        Vector3 force = Vector3.zero;
        //GameObject spawnedObject = Instantiate(prefab);
        //NetworkObject spawnedNetwork = spawnedObject.GetComponent<NetworkObject>();
        //spawnedNetwork.Spawn();
        if (IsServer) { SpawnByServer(index, position, rotation, force); }
        else { SpawnServerRpc(index, position, rotation, force); }
    }

/*
    public void InstantiateOnServer(GameObject prefab){
        Vector3 position = new Vector3(0f, 0f, 0f);
        Quaternion rotation = new Quaternion(0f, 0f, 0f, 1);
        int index = PrefabToIndex(prefab);
        //GameObject spawnedObject = Instantiate(prefab);
        if (IsServer) { SpawnByServer(index, position, rotation); }
        else { SpawnServerRpc(index, position, rotation); }
    }
*/

    [ServerRpc(RequireOwnership = false)]
    private void SpawnServerRpc(int index, Vector3 position, Quaternion rotation, Vector3 force){
        Debug.Log("through rpc");
        SpawnByServer(index, position, rotation, force);
    }

    public void SpawnByServer(int index, Vector3 position, Quaternion rotation, Vector3 force){
        Debug.Log("Getting spawned");
        GameObject prefab = IndexToPrefab(index);
        GameObject spawnedObject = Instantiate(prefab, position, rotation);  
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.Impulse);
        NetworkObject instanceNetworkObject = spawnedObject.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
        lastSpawnedGameObject = spawnedObject;
        SpawnByServerClientRpc(instanceNetworkObject);
        //GameObject spawnedObject = Instantiate(prefab, position, rotation).GetComponent<NetworkObject>().Spawn();
        
        Debug.Log("before");
        
    }
    
    [ClientRpc]
    public void SpawnByServerClientRpc(NetworkObjectReference networkObjectReference){
        Debug.Log("Client has gotten var");
        networkObjectReference.TryGet(out NetworkObject targetObject);
        lastSpawnedGameObject = targetObject.transform.gameObject;
    }
}
