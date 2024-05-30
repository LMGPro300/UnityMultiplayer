using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyInfoUpdate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI maxPlayersText;
    [SerializeField] private Button joinLobbyButton;
    private string lobbyId;

    void Awake(){
        joinLobbyButton.onClick.AddListener(JoinLobby);
    }

    async void JoinLobby(){
        await LobbyManagerUI.Instance.JoinLobbyById(lobbyId);
    }

    public void SetLobbyInfo(string lobbyName, int maxPlayers, int currentPlayers, string lobbyId){
        lobbyNameText.text = lobbyName;
        maxPlayersText.text = currentPlayers + "/" + maxPlayers;
        this.lobbyId = lobbyId;        
    }
}
