using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    
    public void SetLobbyName(string lobbyName){
        lobbyNameText.text = lobbyName;
    }
}
