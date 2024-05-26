using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eflatun.SceneReference;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class Loader{
    public static void LoadScene(SceneReference scene){
        NetworkManager.Singleton.SceneManager.LoadScene(scene.Name, LoadSceneMode.Single);
    }
}
