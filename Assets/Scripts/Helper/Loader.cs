using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eflatun.SceneReference;
using Unity.Netcode;
using UnityEngine.SceneManagement;
/*
 * Program name: Loader.cs
 * Author: Elvin Shen 
 * What the program does: Loads scenes through network
 */
public static class Loader{
    public static void LoadScene(SceneReference scene){
        NetworkManager.Singleton.SceneManager.LoadScene(scene.Name, LoadSceneMode.Single);
    }
}
