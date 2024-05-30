using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoUpdate : MonoBehaviour
{
    private TextMeshProUGUI playerNameText;

    public void Awake(){
        playerNameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void SetPlayerName(string playerName){
        playerNameText.text = playerName;
    }

}
