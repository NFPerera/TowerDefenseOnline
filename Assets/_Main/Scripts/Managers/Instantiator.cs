using System;
using _Main.Scripts.BaseGame.Controllers;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class Instantiator : NetworkBehaviour
    {
        [SerializeField] private NetworkObject waveController;
        //[SerializeField] private NetworkObject chatManager;
        //[SerializeField] private Transform canvasTransform;
        private void Start()
        {

            if (!IsServer)
            {
                MasterManager.Instance.RequestSpawnPlayerDicServerRpc(NetworkManager.Singleton.LocalClientId);
                
            }
            else
            {
                Debug.Log($"SpawnDel waveecontroller");
                var ins = Instantiate(waveController);
                ins.Spawn();
                MasterManager.Instance.SetWaveController(ins.GetComponent<WaveController>());
                // var insChatBox = Instantiate(chatManager, canvasTransform);
                // insChatBox.Spawn();
                // MasterManager.Instance.SetChatManager(insChatBox.GetComponent<ChatManager>());
            }
                
            
        }
    }
}