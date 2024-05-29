using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;


[System.Serializable]
public enum EncryptionType{
    DTLS, //Datagram Transport Layer Security
    WSS //Web Socket Secure
    //UDP and WS exist
}

public class LobbyAuth : MonoBehaviour
{
    [SerializeField] private string lobbyName = "myLobby";
    [SerializeField] private int maxPlayers = 4;
    [SerializeField] EncryptionType encryption = EncryptionType.DTLS;

    string connectionType => encryption == EncryptionType.DTLS ? k_dtlsEncryption : k_wssEncryption;

    public string PlayerId { get; private set; }
    public string PlayerName { get; private set; }

    public static LobbyAuth Instance { get; private set; }

    private Lobby currentLobby;

    const float v_lobbyHeartbeatInterval = 20f;
    const float v_lobbyPollInterval = 65f;
    const string v_keyJoinCode = "RelayJoinCode";
    const string k_dtlsEncryption = "dtls"; 
    const string k_wssEncryption = "wss";
    
    CountdownTimer heartbeatTimer = new CountdownTimer(v_lobbyHeartbeatInterval);
    CountdownTimer pollForUpdatesTimer = new CountdownTimer(v_lobbyPollInterval);

    async void Start(){
        Instance = this;
        DontDestroyOnLoad(this);

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

    async Task Authenticate(){
        await Authenticate("Player" + Random.Range(0, 1000));
    }

    async Task Authenticate(string playerName){
        if (UnityServices.State == ServicesInitializationState.Uninitialized){
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(playerName);

            await UnityServices.InitializeAsync(options);
        }

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
        };

        if(!AuthenticationService.Instance.IsSignedIn) {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerId = AuthenticationService.Instance.PlayerId;
            PlayerName = playerName;
        }
    }

    public async Task CreateRelay(){
        try{
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, connectionType));
            NetworkManager.Singleton.StartHost();
        } catch (RelayServiceException e){
            Debug.LogError("Failed to create RELAY " + e.Message);
        }
    }

    public async Task JoinRelay2(string relayJoinCode){
        try{
            pollForUpdatesTimer.Start();
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, connectionType));
            NetworkManager.Singleton.StartClient();
        } catch (RelayServiceException e){
            Debug.LogError("Failed to join RELAY " + e.Message);
        }
    }

    public async Task CreateLobby(){
        try{
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            CreateLobbyOptions options = new CreateLobbyOptions{
                IsPrivate = false
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            Debug.Log("Created Lobby: " + currentLobby.Name + " with code " + currentLobby.LobbyCode);

            //heartbeat
            heartbeatTimer.Start();
            pollForUpdatesTimer.Start();

            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    {v_keyJoinCode, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, connectionType));

            NetworkManager.Singleton.StartHost();


        } catch (LobbyServiceException e){
            Debug.LogError("Failed to create lobby: " + e.Message);
        }
    }

    public async Task QuickJoinLobby(){
        try{
            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            pollForUpdatesTimer.Start();

            string relayJoinCode = currentLobby.Data[v_keyJoinCode].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, connectionType));

            NetworkManager.Singleton.StartClient();

        } catch (LobbyServiceException e){
            Debug.LogError("Failed to quick join lobby: " + e.Message);
        }
    }


    async Task<Allocation> AllocateRelay(){
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            return allocation;
        } catch (RelayServiceException e){
            Debug.LogError("Failed to allocate relay: " + e.Message);
            return default;
        }
    }

    async Task<string> GetRelayJoinCode(Allocation allocation){
        try{
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        } catch (RelayServiceException e){
            Debug.LogError("Failed to get relay join code: " + e.Message);
            return default;
        }
    }

    async Task<JoinAllocation> JoinRelay(string relayJoinCode){
        try{
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            return joinAllocation;
        } catch(RelayServiceException e){
            Debug.LogError("Failed to join relay: " + e.Message);
            return default;
        }
    }

    async Task HandleHeartbeatAsync(){
        try{
            await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            Debug.Log("Sent heatbeat ping to lobby " + currentLobby.Name);
        } catch (LobbyServiceException e){
            Debug.LogError("Failed to heartbeat lobby: " + e.Message);
        }
    }

    async Task HandlePollForUpdateAsync(){
        try{
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
            Debug.Log("Polled for update on lobby " + lobby.Name);
        } catch (LobbyServiceException e){
            Debug.LogError("Failed to poll for updates on lobby: " + e.Message);
        }
    }
}
