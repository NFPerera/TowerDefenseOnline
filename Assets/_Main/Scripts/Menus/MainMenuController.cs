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
        [SerializeField] private NetWorkController controller;

        [Header("MainMenu")] 
        [SerializeField] private GameObject mainMenuObj;

        [SerializeField] private GameObject creditsScreenObj;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button serverButton;
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
            NetworkManager.Singleton.OnTransportFailure -= OnFailedConnection;
        }

#region Buttons

        public void OnHostEventHandler()
        {
            NetworkManager.Singleton.OnServerStarted += OnWaitingRoomEnable;
            controller.OnHost();
            SetInteractableButtons(false);
        }

        public void OnServerEventHandler()
        {
            NetworkManager.Singleton.OnServerStarted += OnWaitingRoomEnable;
            controller.OnServer();
            SetInteractableButtons(false);
        }

        public void OnClientEventHandler()
        {
            NetworkManager.Singleton.OnClientStarted += OnWaitingRoomEnable;
            controller.OnClient();
            SetInteractableButtons(false);
        }

        public void OnStartGameButtonClicked()
        {
            controller.ChangeNetScene(sceneToLoad);
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

        public void OnCharacterSelection(int charId)
        {
            //var instance = GameManager.Instance;
            //var data = instance.GetPlayersData();
            
            //instance.SetPlayerSpawnData(data[charId], charId);
            //roomManager.OnPlayerJoinRoom(data[charId], NetworkManager.Singleton.LocalClientId);
            OnWaitingRoomEnable();
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