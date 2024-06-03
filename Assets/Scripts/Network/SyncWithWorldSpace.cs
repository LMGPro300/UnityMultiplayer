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

    private int PrefabToIndex(GameObject prefab){
        return prefabNetworkList._prefabNetworkList.IndexOf(prefab);
    }
    
    private GameObject IndexToPrefab(int index){
        return prefabNetworkList._prefabNetworkList[index];
    }
    
    public void DestoryOnServer(GameObject obj){
        DestoryOnServerServerRpc(obj.GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestoryOnServerServerRpc(NetworkObjectReference networkObjectReference){
        networkObjectReference.TryGet(out NetworkObject obj);
        GameObject actualObj = obj.transform.gameObject;
        Debug.Log("GOT GONED LLL");
        Destroy(actualObj);
    }

    public void InstantiateOnServer(GameObject prefab, Vector3 position, Quaternion rotation){
        int index = PrefabToIndex(prefab);
        //GameObject spawnedObject = Instantiate(prefab);
        if (IsServer) { SpawnByServer(index, position, rotation); }
        else { SpawnServerRpc(index, position, rotation); }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnServerRpc(int index, Vector3 position, Quaternion rotation){
        Debug.Log("through rpc");
        SpawnByServer(index, position, rotation);
    }

    public void SpawnByServer(int index, Vector3 position, Quaternion rotation){
        Debug.Log("Getting spawned");
        GameObject prefab = IndexToPrefab(index);
        GameObject spawnedObject = Instantiate(prefab);
        NetworkObject instanceNetworkObject = spawnedObject.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
        //GameObject spawnedObject = Instantiate(prefab, position, rotation).GetComponent<NetworkObject>().Spawn();
        lastSpawnedGameObject = spawnedObject;
        Debug.Log("before");
        SpawnByServerClientRpc(instanceNetworkObject);
    }
    
    [ClientRpc]
    public void SpawnByServerClientRpc(NetworkObjectReference networkObjectReference){
        Debug.Log("Client has gotten var");
        networkObjectReference.TryGet(out NetworkObject targetObject);
        lastSpawnedGameObject = targetObject.transform.gameObject;
    }
}
