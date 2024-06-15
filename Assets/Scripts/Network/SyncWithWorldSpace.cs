using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/*
 * Program name: SyncWithWorldSpace.cs
 * Author: Elvin Shen 
 * What the program does: Helper network that can spawn prefabs into the network 
 */
public class SyncWithWorldSpace: NetworkBehaviour
{
    public static SyncWithWorldSpace Instance { get; private set; }

    [SerializeField] private PrefabNetworkList prefabNetworkList;
    public GameObject lastSpawnedGameObject;

    public override void OnNetworkSpawn(){
        if (!IsServer) return;
        base.OnNetworkSpawn();
        Instance = this;
    }

    //assign a prefab to an index since cannot directly pass prefab to rpc
    public int PrefabToIndex(GameObject prefab){
        return prefabNetworkList._prefabNetworkList.IndexOf(prefab);
    }
    
    //convert index to prefab
    public GameObject IndexToPrefab(int index){
        return prefabNetworkList._prefabNetworkList[index];
    }
    
    //destory a network object on the network
    public void DestoryOnServer(GameObject obj){
        if (obj == null) return;
        NetworkObject nObj = obj.GetComponent<NetworkObject>();
        if (nObj == null || !nObj.IsSpawned) return;
        DestoryOnServerServerRpc(nObj);
    }

    //despawn the object in the network
    [ServerRpc(RequireOwnership = false)]
    private void DestoryOnServerServerRpc(NetworkObjectReference networkObjectReference){
        networkObjectReference.TryGet(out NetworkObject obj);
        if (obj == null) return;
        Debug.Log("GOT GONED LLL");
        obj.Despawn(true);
    }

    //Spawn an object on the server, the client can only request to spawn an object in
    public void InstantiateOnServer(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 force){
        int index = PrefabToIndex(prefab);
        //GameObject spawnedObject = Instantiate(prefab);
        //NetworkObject spawnedNetwork = spawnedObject.GetComponent<NetworkObject>();
        //spawnedNetwork.Spawn();
        if (IsServer) { SpawnByServer(index, position, rotation, force); }
        else { SpawnServerRpc(index, position, rotation, force); }
    }

    //same thing but without a force value
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

    //Client requests to spawn an object
    [ServerRpc(RequireOwnership = false)]
    private void SpawnServerRpc(int index, Vector3 position, Quaternion rotation, Vector3 force){
        Debug.Log("through rpc");
        SpawnByServer(index, position, rotation, force);
    }

    //spawn an object on the server, apply position, rotation and rigidbody force
    public void SpawnByServer(int index, Vector3 position, Quaternion rotation, Vector3 force){
        Debug.Log("Getting spawned");
        GameObject prefab = IndexToPrefab(index);
        GameObject spawnedObject = Instantiate(prefab, position, rotation);  
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb != null){
            rb.AddForce(force, ForceMode.Impulse);
        }
        NetworkObject instanceNetworkObject = spawnedObject.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
        lastSpawnedGameObject = spawnedObject;
        SpawnByServerClientRpc(instanceNetworkObject);
        //GameObject spawnedObject = Instantiate(prefab, position, rotation).GetComponent<NetworkObject>().Spawn();
        
        Debug.Log("before"); 
    }
    
    //Server respond to request to spawn an object, clients tries to get information
    [ClientRpc]
    public void SpawnByServerClientRpc(NetworkObjectReference networkObjectReference){
        Debug.Log("Client has gotten var");
        networkObjectReference.TryGet(out NetworkObject targetObject);
        if (targetObject != null){
            lastSpawnedGameObject = targetObject.transform.gameObject;
        }
        
    }
    
    //Same as InstantiateOnServer, but creates a unique item that has unique traits to it
    public void InstantiateItemOnServer(NetworkObject playerReference, GameObject prefab, GunPayload gunPayload, Vector3 position, Quaternion rotation, Vector3 force){
        int index = PrefabToIndex(prefab);
        //GameObject spawnedObject = Instantiate(prefab);
        //NetworkObject spawnedNetwork = spawnedObject.GetComponent<NetworkObject>();
        //spawnedNetwork.Spawn();
        if (IsServer) { SpawnItemByServer(playerReference, index, gunPayload, position, rotation, force); }
        else { SpawnItemServerRpc(playerReference, index, gunPayload, position, rotation, force); }
    }

    //request to spawn an item on the network
    [ServerRpc(RequireOwnership = false)]
    private void SpawnItemServerRpc(NetworkObjectReference playerReference, int index, GunPayload gunPayload, Vector3 position, Quaternion rotation, Vector3 force){
        SpawnItemByServer(playerReference, index, gunPayload, position, rotation, force);
    }

    //assign the regular infomation and include item informations to the server
    public void SpawnItemByServer(NetworkObjectReference playerReference, int index, GunPayload gunPayload, Vector3 position, Quaternion rotation, Vector3 force){
        playerReference.TryGet(out NetworkObject playerObj);
        GameObject shopChild = playerObj.transform.Find("Enemy Stop Zone").gameObject;
        Debug.Log(shopChild.name + " I got this child LOL");
        ShopManager shopManager = shopChild.GetComponent<ShopManager>();
        
        
        GameObject prefab = IndexToPrefab(index);
        GameObject spawnedObject = Instantiate(prefab, position, rotation);  
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.Impulse);
        ItemObject itemObj = spawnedObject.GetComponent<ItemObject>();
        if (gunPayload.magSize != 0){
            itemObj.referenceItem.weapon.SetNewData(gunPayload);
        }

        itemObj.lastOwner = shopManager;

        NetworkObject instanceNetworkObject = spawnedObject.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();

        lastSpawnedGameObject = spawnedObject;
        SpawnByServerClientRpc(instanceNetworkObject);
    }
}
