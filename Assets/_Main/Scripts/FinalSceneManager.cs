using System;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts
{
    public class FinalSceneManager : NetworkBehaviour
    {
        [SerializeField] private String mainMenuScene;
        
        
        public void OnBackToMenu()
        {
            MasterManager.Instance.RequestDisconnectUserServerRpc(NetworkManager.Singleton.LocalClientId);

            SceneManager.LoadScene(mainMenuScene);
        }
        
        // public void OnBackToMenu()
        // {
        //     MasterManager.Instance.RequestDisconnectUserServerRpc(NetworkManager.Singleton.LocalClientId);
        //     MasterManager.Instance.ChangeNetScene(mainMenuScene);
        // }
        
        
        public void OnQuit()
        {
            Application.Quit();
        }
        
        // public void OnQuit()
        // {
        //     MasterManager.Instance.RequestDisconnectUserServerRpc(NetworkManager.Singleton.LocalClientId);
        //     Application.Quit();
        // }
    }
}
