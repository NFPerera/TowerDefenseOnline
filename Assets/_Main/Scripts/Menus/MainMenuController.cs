using System.Runtime.CompilerServices;
using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.Managers;
using _Main.Scripts.Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static _Main.Scripts.Managers.GameNetworkManager;

namespace _Main.Scripts.Menus
{
    public class MainMenuController : MonoBehaviour
    {

        [Header("MainMenu")] 
        [SerializeField] private GameObject mainMenuObj;

        [SerializeField] private GameObject creditsScreenObj;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button serverButton;
        [SerializeField] private Button clientButton;
        [SerializeField] private TMP_InputField inputFieldName;

        [Header("RoomSetting")] 
        [SerializeField] private RoomManager roomManager;
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
            NetworkManager.Singleton.OnTransportFailure -= OnFailedConnection;
        }

#region Buttons

        public void OnHostEventHandler()
        {
            NetworkManager.Singleton.OnServerStarted += OnWaitingRoomEnable;
            MasterManager.Instance.OnHost();
            SetInteractableButtons(false);
        }

        public void OnServerEventHandler()
        {
            NetworkManager.Singleton.OnServerStarted += OnWaitingRoomEnable;
            MasterManager.Instance.OnServer();
            SetInteractableButtons(false);
        }

        public void OnClientEventHandler()
        {
            NetworkManager.Singleton.OnClientStarted += OnWaitingRoomEnable;
            MasterManager.Instance.OnClient();
            roomManager.RequestPlayerJoinRoomUpdateServerRpc(NetworkManager.Singleton.LocalClientId, playersName);
            SetInteractableButtons(false);
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
            var id = NetworkManager.Singleton.LocalClientId;
            MasterManager.Instance.SetPlayersNameServerRpc(id, playersName);
        }


        private void OnFailedConnection()
        {
            SetInteractableButtons(true);
        }
        private void SetInteractableButtons(bool p_b)
        {
            hostButton.enabled = p_b;
            clientButton.enabled = p_b;
            serverButton.enabled = p_b;
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
        }
        
    }
}