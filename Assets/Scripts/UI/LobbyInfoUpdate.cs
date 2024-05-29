using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyInfoUpdate : MonoBehaviour
{
    private TextMeshProUGUI lobbyNameText;
    private TextMeshProUGUI maxPlayersText;

    public void Awake(){
        Transform child0 = transform.GetChild(0);
        Debug.Log(child0.name);
        TextMeshProUGUI childt0 = child0.GetComponent<TextMeshProUGUI>();
        Debug.Log(childt0);
        lobbyNameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        maxPlayersText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
    public void SetLobbyInfo(string lobbyName, int maxPlayers, int currentPlayers){
        lobbyNameText.text = lobbyName;
        maxPlayersText.text = currentPlayers + "/" + maxPlayers;
    }
}
