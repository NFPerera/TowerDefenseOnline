using System.Collections;
using _Main.Scripts.Managers;
using _Main.Scripts.Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Menus
{
    public class MainMenuController : MonoBehaviour
    {

        [Header("Server Managers")] 
        [SerializeField] private NetworkObject managerObj;
        [Header("MainMenu")] 
        [SerializeField] private GameObject mainMenuObj;

        [SerializeField] private GameObject creditsScreenObj;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;
        [SerializeField] private TMP_InputField inputFieldName;

        [Header("RoomSetting")] 
        [SerializeField] private GameObject waitingRoomGameObject;
        [SerializeField] private GameObject startGameButton;
        [SerializeField] private string playersName = "Carlitos";
        
        
        [Header("StartGameSetting")]
        [SerializeField] private string sceneToLoad;
        
        private void Start()
        {
            var manager = NetworkManager.Singleton;
            
            manager.OnTransportFailure += OnFailedConnection;
            
            
        }

        private void OnDestroy()
        {
            //NetworkManager.Singleton.OnTransportFailure -= OnFailedConnection;
        }

#region Buttons

        public void OnHostEventHandler()
        {
            NetworkManager.Singleton.OnServerStarted += SpawnServerManagers;
            ///MasterManager.Instance.OnHost();
            NetworkManager.Singleton.StartHost();
            //Crear otro metodo para el server started que cree el MasterManager
            SetInteractableButtons(false);
        }

        public void OnClientEventHandler()
        {
            NetworkManager.Singleton.OnClientStarted += OnWaitingRoomEnable;
            NetworkManager.Singleton.StartClient();
            Debug.Log($"Button Pressed, id {NetworkManager.Singleton.LocalClientId}");
            
            
            SetInteractableButtons(false);
        }


        private void SpawnServerManagers()
        {
            mainMenuObj.SetActive(false);
            waitingRoomGameObject.SetActive(true);
            var manager = NetworkManager.Singleton;
            startGameButton.SetActive(manager.IsHost || manager.IsServer);


            var obj = Instantiate<NetworkObject>(managerObj);
            obj.Spawn();
        }
        public void OnStartGameButtonClicked()
        {
            MasterManager.Instance.ChangeNetScene(sceneToLoad);
        }

        public void OnCreditsButtonClicked()
        {
            creditsScreenObj.SetActive(true);
        }

        public void OnBackButtonClicked()
        {
            creditsScreenObj.SetActive(false);
        }

        public void OnExitButtonClicked()
        {
            Application.Quit();
        }
        
#endregion
        

        public void OnChangeUserName()
        {
            playersName = inputFieldName.text;
            PlayerPrefs.SetString("Nickname", playersName);
            // var id = NetworkManager.Singleton.LocalClientId;
            // MasterManager.Instance.SetPlayersNameServerRpc(id, playersName);
        }


        private void OnFailedConnection()
        {
            SetInteractableButtons(true);
        }
        private void SetInteractableButtons(bool p_b)
        {
            hostButton.enabled = p_b;
            clientButton.enabled = p_b;
        }

        private void OnMainMenuEnable()
        {
            mainMenuObj.SetActive(true);
            waitingRoomGameObject.SetActive(false);
        }
        

        private void OnWaitingRoomEnable()
        {
            mainMenuObj.SetActive(false);
            waitingRoomGameObject.SetActive(true);
            var manager = NetworkManager.Singleton;
            startGameButton.SetActive(manager.IsHost || manager.IsServer);
            
            Debug.Log($"Conecting. . . {NetworkManager.Singleton.LocalClientId}");
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnectionToServer;
        }

        private void OnConnectionToServer(ulong obj)
        {
            if(obj == NetworkManager.Singleton.LocalClientId)
                MasterManager.Instance.RequestPlayerJoinRoomUpdateServerRpc(NetworkManager.Singleton.LocalClientId, playersName);
        }


    }
}