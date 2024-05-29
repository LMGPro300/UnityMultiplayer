using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Eflatun.SceneReference;

using Unity.Services.Lobbies.Models;

public class LobbyManagerUI : MonoBehaviour
{
    [SerializeField] private SceneReference gameScene;
    [SerializeField] private GameObject lobbyListPanel;
    [SerializeField] private GameObject newLobbyPanel;
    [SerializeField] private TestLobby testLobby;

    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button reloadLobbyButton;
    [SerializeField] private Button launchLobbyButton;

    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private GameObject lobbyPrefab;
    [SerializeField] private int lobbySpacing;
    
    private string playerName;
    private string lobbyName = "myLobby";
    private int lobbyMaxPlayers = 4;

    void Awake(){
        lobbyListPanel.SetActive(true);
        newLobbyPanel.SetActive(false);
        launchLobbyButton.onClick.AddListener(CreateLobby);
        createLobbyButton.onClick.AddListener(CreateNewLobbyUI);
        reloadLobbyButton.onClick.AddListener(ReloadLobbyList);
    }

    public void GetPlayerName(string input){
        playerName = input;
    }
    public void GetLobbyName(string input){
        lobbyName = input;
    }
    public void GetLobbyMaxPlayers(string input){
        lobbyMaxPlayers = int.Parse(input);
    }

    async void CreateLobby(){
        lobbyListPanel.SetActive(true);
        newLobbyPanel.SetActive(false);
        Debug.Log("got max players " + lobbyMaxPlayers);
        await testLobby.CreateLobby(lobbyName, lobbyMaxPlayers);
        //Loader.LoadScene(gameScene);
    }

    void CreateNewLobbyUI(){
        lobbyListPanel.SetActive(false);
        newLobbyPanel.SetActive(true);
    }

    async void ReloadLobbyList(){
        List<Lobby> lobbies = await testLobby.GetListLobbies();
        int offset = 0;
        Debug.Log("Found " + lobbies.Count + " lobbies!");

        foreach(Transform child in lobbyContainer){
            Destroy(child.gameObject);
        }
        foreach(Lobby lobby in lobbies){
            Debug.Log("Found lobby " + lobby.Name);
            GameObject newLobbyUI = Instantiate(lobbyPrefab, lobbyContainer);
            newLobbyUI.gameObject.SetActive(true);
            newLobbyUI.transform.position = new Vector3(newLobbyUI.transform.position.x, newLobbyUI.transform.position.y + offset, newLobbyUI.transform.position.z);
            offset -= lobbySpacing;

            LobbyInfoUpdate lobbyInfoUpdate = newLobbyUI.GetComponent<LobbyInfoUpdate>();
            lobbyInfoUpdate.SetLobbyInfo(lobby.Name, lobby.MaxPlayers, lobby.Players.Count);
        }
    }

    async void ReloadPlayerList(){

        
    }

}
