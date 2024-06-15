using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Core;
using Unity.Services.Authentication;

/*
 * Program name: TestLobby.cs
 * Author: Elvin Shen (not really)
 * What the program does: Creates lobbies and makes them pulbic to join
 * CREDITS: https://www.youtube.com/watch?v=7glCsF9fv3s
 */


public class TestLobby : MonoBehaviour
{   
    private Lobby hostLobby;
    public Lobby joinnedLobby;
    [SerializeField] LobbyAuth lobbyAuth;
    public string PlayerId { get; private set; }
    public string PlayerName { get; private set; }

    const float v_lobbyHeartbeatInterval = 20f;

    const float v_lobbyPollInterval = 5f;
    public CountdownTimer heartbeatTimer = new CountdownTimer(v_lobbyHeartbeatInterval);
    public CountdownTimer pollForUpdatesTimer = new CountdownTimer(v_lobbyPollInterval);

    //Authicate the join and define countdown timers
    private async void Start(){
        await Authenticate();
        
        heartbeatTimer.OnTimerStop += async () => {
            await HandleHeartbeatAsync();
            heartbeatTimer.Start();
        };

        pollForUpdatesTimer.OnTimerStop += async () => {
            await HandlePollForUpdateAsync();
            pollForUpdatesTimer.Start();
        };
    }

    void Update(){
        heartbeatTimer.Tick(Time.deltaTime);
        pollForUpdatesTimer.Tick(Time.deltaTime);
    }
    //Give a random player ID
    async Task Authenticate(){
        await Authenticate("Player" + Random.Range(0, 1000));
    }
    //If the player already exists, register them
    async Task Authenticate(string playerName){
        if (UnityServices.State == ServicesInitializationState.Uninitialized){
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(playerName);

            await UnityServices.InitializeAsync(options);
        }

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
        };
        //if not exists, regiester as guest
        if(!AuthenticationService.Instance.IsSignedIn) {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerId = AuthenticationService.Instance.PlayerId;
            PlayerName = playerName;
        }
    }

    //Create a lobby and start the heartbeat to keep it up
    public async Task CreateLobby(string lobbyName, int maxPlayers){
        try{
            CreateLobbyOptions options = new CreateLobbyOptions{
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>{
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, "CaptureTheFlag")},
                    {"Map", new DataObject(DataObject.VisibilityOptions.Public, "de_dust2")},
                    {"RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            Debug.Log("Make lobby with code: " + lobby.LobbyCode);
            hostLobby = lobby;
            joinnedLobby = hostLobby;
            heartbeatTimer.Start();
        }catch (LobbyServiceException e){
            Debug.LogError("Failed to create lobby: " + e.Message);
        }
    }

    //list the lobbies avaliable to join
    private async void ListLobbies(){
        try{
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results){
                Debug.Log(lobby.Name + " with " + lobby.MaxPlayers + " max players and " + lobby.Data["GameMode"].Value + " gamemode and map " + lobby.Data["Map"].Value);
            }
        } catch(LobbyServiceException e){
            Debug.LogError("Failed to list lobbies " + e.Message);
        }
    }

    //Get a List of lobbies and return it
    public async Task<List<Lobby>> GetListLobbies(){
        try{
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            return queryResponse.Results;
        } catch (LobbyServiceException e){
            
            Debug.LogError("Failed to get list of lobbies " + e.Message);
        }
        return new List<Lobby>();
    }

    //print the list of players in the lobby
    public void ListPlayers(Lobby lobby){
        Debug.Log("Players in lobby " + lobby.Name);

        foreach(Player player in lobby.Players){
            Debug.Log(player.Data["PlayerName"].Value);
        }
    }

    //Join the lobby by a code and register them as a player
    public async Task JoinLobby(string lobbyCode){
        try{
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions{
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinnedLobby = lobby;
            await HandlePollForUpdateAsync();
            pollForUpdatesTimer.Start();
        } catch(LobbyServiceException e){
            joinnedLobby = null;
            Debug.LogError("Failed to join lobby " + e.Message);
        }
    }

    //literally no difference i dont know why I have this here
    public async Task JoinLobbyById(string lobbyId){
        try{
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions{
                Player = GetPlayer()
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
            joinnedLobby = lobby;
            await HandlePollForUpdateAsync();
            pollForUpdatesTimer.Start();
        } catch(LobbyServiceException e){
            joinnedLobby = null;
            Debug.LogError("Failed to join lobby " + e.Message);
        }
    }

    //Quick join a lobby
    private async void QuickJoinLobby(string lobbyCode){
        try{
            await LobbyService.Instance.QuickJoinLobbyAsync();
            
        } catch(LobbyServiceException e){
            Debug.LogError("Failed to quick join lobby " + e.Message);
        }
    }

    //leave a lobby
    private async void LeaveLobby(){
        try{
            await LobbyService.Instance.RemovePlayerAsync(joinnedLobby.Id, AuthenticationService.Instance.PlayerId);
        } catch(LobbyServiceException e){
            Debug.LogError("Failed to leave lobby " + e.Message);
        }
    }

    //host kicked a player in lobby
    private async void KickPlayer(Player player){
        try{
            await LobbyService.Instance.RemovePlayerAsync(joinnedLobby.Id, player.Id);
        } catch (LobbyServiceException e){
            Debug.LogError("Failed to kick player " + e.Message);
        }
    }

    //Get the player information
    private Player GetPlayer(){
        return new Player{
                    Data = new Dictionary<string, PlayerDataObject>{
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerName)}
                    }
                };
    }

    //Update the gamemode of the lobby and send to lobby service
    private async void UpdateLobbyGameMode(string gameMode){
        try{
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>{
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)}
                }
            });
            joinnedLobby = hostLobby;
        } catch(LobbyServiceException e){
            Debug.LogError("Failed to edit lobby gamemode " + e.Message);
        }
    }

    //Update the playnames
    public async Task UpdatePlayerName(string newName){
        try{
            await LobbyService.Instance.UpdatePlayerAsync(joinnedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions{
                Data = new Dictionary<string, PlayerDataObject>{
                    {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newName)}
                }
            });
        } catch(LobbyServiceException e){
            Debug.LogError("Failed update player name " + e.Message);
        }
    }

    //heartbeat the lobby to keep it showing
    async Task HandleHeartbeatAsync(){
        try{
            await LobbyService.Instance.SendHeartbeatPingAsync(joinnedLobby.Id);
            Debug.Log("Sent heatbeat ping to lobby " + joinnedLobby.Name);
        } catch (LobbyServiceException e){
            Debug.LogError("Failed to heartbeat lobby: " + e.Message);
        }
    }
    
    //Get updates on the lobby, if the host started, start the game for the clients
    public async Task HandlePollForUpdateAsync(){
        try{
            if (joinnedLobby == null) return;
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinnedLobby.Id);
            joinnedLobby = lobby;

            if(joinnedLobby.Data["RelayJoinCode"].Value != "0"){
                if (PlayerId == joinnedLobby.HostId) return;
                Debug.Log(joinnedLobby);
                Debug.Log(joinnedLobby.Data["RelayJoinCode"].Value);
                await lobbyAuth.JoinRelay2(joinnedLobby.Data["RelayJoinCode"].Value);

                joinnedLobby = null;
            }
            Debug.Log("Polled for update on lobby " + lobby.Name);
        } catch (LobbyServiceException e){
            Debug.LogError("Failed to poll for updates on lobby: " + e.Message);
        }
    }
}
