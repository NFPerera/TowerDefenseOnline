using System;
using System.Collections;
using System.Collections.Generic;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndgameMenuManager : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    public void OnMainMenuButtonClicked()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
    
    
    public void OnExitButtonClicked()
    {
        Application.Quit();
    }
}
