using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Eflatun.SceneReference;

public class LobbyManagerUI : MonoBehaviour
{
    [SerializeField] Button createLobbyButton;
    [SerializeField] Button quickJoinButton;
    [SerializeField] SceneReference gameScene;

    void Awake(){
        createLobbyButton.onClick.AddListener(CreateLobby);
        quickJoinButton.onClick.AddListener(QuickJoinGame);
    }

    async void CreateLobby(){
        await LobbyAuth.Instance.CreateLobby();
        Loader.LoadScene(gameScene);
    }

    async void QuickJoinGame(){
        await LobbyAuth.Instance.QuickJoinLobby();
    }
}
