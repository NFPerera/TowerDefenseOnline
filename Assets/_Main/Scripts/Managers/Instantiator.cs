using System;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class Instantiator : NetworkBehaviour
    {
        [SerializeField] private NetworkObject waveController;
        private void Start()
        {

            if (!IsServer)
            {
                MasterManager.Instance.RequestSpawnPlayerDicServerRpc(NetworkManager.Singleton.LocalClientId);
                
            }
            else
            {
                var ins = Instantiate(waveController);
                ins.Spawn();
                MasterManager.Instance.SearchWaveController();
            }
                
            
        }
    }
}