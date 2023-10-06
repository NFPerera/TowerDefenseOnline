using _Main.Scripts.Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.Networking
{
    public class NetWorkController : MonoBehaviour
    {
        

        private void OnOnClientConnected(ulong p_data)
        {
            Debug.Log("Client Connected: " + p_data);
        }

        private void OnServerStarted()
        {
            Debug.Log("ServerStarted");
        }

        


        public void OnHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void OnServer()
        {
            var instance = NetworkManager.Singleton;
            instance.OnServerStarted += OnServerStarted;
            
            instance.StartServer();
        }

        public void OnClient()
        {
            var instance = NetworkManager.Singleton;
            instance.OnClientConnectedCallback += OnOnClientConnected;
            instance.StartClient();
        }

        


        public void ChangeNetScene(string p_sceneName)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(p_sceneName, LoadSceneMode.Single);
            // GameNetworkManager.TriggerGameSceneLoaded();
        }
        
        public void AddNetScene(string p_sceneName)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(p_sceneName, LoadSceneMode.Additive);
        }
        
    }
}
