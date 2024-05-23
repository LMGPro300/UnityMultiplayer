using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    
    private void Awake(){
        hostButton.onClick.AddListener(() => {NetworkManager.Singleton.StartHost();});
        //hostButton.onClick.AddListener(() => {Debug.Log("Clicked host button");});
        clientButton.onClick.AddListener(() => {NetworkManager.Singleton.StartClient();});
    }
}
