using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Eflatun.SceneReference;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies.Models;
/*
 * Program name: LobbyManagerUI.cs
 * Author: Elvin Shen 
 * What the program does: The main controller of the lobby UI
 */
public class LobbyManagerUI : MonoBehaviour
{
    public static LobbyManagerUI Instance { get; private set; }
    [SerializeField] private SceneReference gameScene;
    [SerializeField] private TestLobby testLobby;
    [SerializeField] private LobbyAuth lobbyAuth;
    [Header("UI Panels")]
    [SerializeField] private GameObject lobbyListPanel;
    [SerializeField] private GameObject newLobbyPanel;
    [SerializeField] private GameObject lobbyPanel;
    [Header("Buttons")]
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button reloadLobbyButton;
    [SerializeField] private Button launchLobbyButton;
    [SerializeField] private Button reloadPlayersButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button joinLobbyCodeButton;
    [Header("Containers")]
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform playerContainer;
    [Header("Prefabs")]
    [SerializeField] private GameObject lobbyPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject kickButtonPrefab;
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [Header("Constants")]
    [SerializeField] private int lobbySpacing;
    
    private string playerName;
    private string lobbyName = "myLobby";
    private int lobbyMaxPlayers = 4;
    private string inputLobbyCode;

    //Hide the panels such as "create lobby" or "lobby panel"
    //since the user is currently in the lobby menu
    void Awake(){
        Instance = this;
        lobbyListPanel.SetActive(true);
        newLobbyPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        launchLobbyButton.onClick.AddListener(CreateLobby);
        createLobbyButton.onClick.AddListener(CreateNewLobbyUI);
        reloadLobbyButton.onClick.AddListener(ReloadLobbyList);
        reloadPlayersButton.onClick.AddListener(ReloadPlayerList);
        startGameButton.onClick.AddListener(StartGame);
        joinLobbyCodeButton.onClick.AddListener(JoinLobbyByCode);
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
    public void GetLobbyCode(string input){
        inputLobbyCode = input;
    }

    //create a lobby and show the create lobby panel
    async void CreateLobby(){
        lobbyListPanel.SetActive(false);
        newLobbyPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        Debug.Log("got max players " + lobbyMaxPlayers);
        lobbyNameText.text = lobbyName;
        
        await testLobby.CreateLobby(lobbyName, lobbyMaxPlayers);
        testLobby.pollForUpdatesTimer.Start();
        lobbyCodeText.text = "Lobby Code: " + testLobby.joinnedLobby.LobbyCode;
        //
    }

    void CreateNewLobbyUI(){
        lobbyListPanel.SetActive(false);
        newLobbyPanel.SetActive(true);
    }

    public async void JoinLobbyByCode(){
        await JoinLobby(inputLobbyCode);
        
    }

    //join a lobby, upon entering a valid lobby, update the players
    //lobby codes and hide the lobby menu
    public async Task JoinLobby(string lobbyCode){
        lobbyListPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        startGameButton.gameObject.SetActive(false);
        await testLobby.JoinLobby(lobbyCode);
        lobbyNameText.text = testLobby.joinnedLobby.Name;
        lobbyCodeText.text = "Lobby Code: " + testLobby.joinnedLobby.LobbyCode;

        if (testLobby.joinnedLobby == null){
            lobbyListPanel.SetActive(true);
            lobbyPanel.SetActive(false);
        }
    }

    //same as JoinLobby but by Id this time
    public async Task JoinLobbyById(string lobbyId){
        lobbyListPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        startGameButton.gameObject.SetActive(false);
        await testLobby.JoinLobbyById(lobbyId);
        lobbyNameText.text = testLobby.joinnedLobby.Name;
        lobbyCodeText.text = "Lobby Code: " + testLobby.joinnedLobby.LobbyCode;

        if (testLobby.joinnedLobby == null){
            lobbyListPanel.SetActive(true);
            lobbyPanel.SetActive(false);
        }
    }

    //The game started, and the game scene is loaded
    public async void StartGame(){
        await lobbyAuth.CreateLobbyRelay(testLobby.joinnedLobby);
        Loader.LoadScene(gameScene);
    }

    //The lobby list is refreshed, for each lobby found a panel is made
    //showing current player count, lobby name, etc.
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
            lobbyInfoUpdate.SetLobbyInfo(lobby.Name, lobby.MaxPlayers, lobby.Players.Count, lobby.Id);
        }
    }

    //refresh the player list, showing and updating everyone's name
    // and if they are currently in lobby
    async void ReloadPlayerList(){
        if (testLobby.joinnedLobby == null) return;
        int offset = 0;
        foreach(Transform child in playerContainer){
            Destroy(child.gameObject);
        }
        foreach(Player player in testLobby.joinnedLobby.Players){
            Debug.Log(playerName);
            if (playerName != null){
                await testLobby.UpdatePlayerName(playerName);
                await testLobby.HandlePollForUpdateAsync();
            }
            GameObject newPlayerUI = Instantiate(playerPrefab, playerContainer);
            newPlayerUI.gameObject.SetActive(true);
            newPlayerUI.transform.position = new Vector3(newPlayerUI.transform.position.x, newPlayerUI.transform.position.y + offset, newPlayerUI.transform.position.z);
            

            PlayerInfoUpdate playerInfoUpdate = newPlayerUI.GetComponent<PlayerInfoUpdate>();
            playerInfoUpdate.SetPlayerName(player.Data["PlayerName"].Value);

            if (testLobby.joinnedLobby.HostId == player.Id){
                GameObject newKickButton = Instantiate(kickButtonPrefab, newPlayerUI.transform);
                newKickButton.gameObject.SetActive(true);
                newKickButton.transform.position = new Vector3(newKickButton.transform.position.x, newKickButton.transform.position.y + offset, newKickButton.transform.position.z);
                
            }
            offset -= lobbySpacing;
        }

        testLobby.ListPlayers(testLobby.joinnedLobby);
    }

}
