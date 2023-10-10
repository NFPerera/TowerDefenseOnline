using System;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class Instantiator : NetworkBehaviour
    {
        [SerializeField] private NetworkObject waveManager;
        private void Start()
        {

            if (NetworkManager.Singleton.IsServer)
            {
                var instance = Instantiate(waveManager);
                instance.Spawn();
            }
            else
            {
                MasterManager.Instance.RequestSpawnPlayerDicServerRpc(NetworkManager.Singleton.LocalClientId);
                
            }
                
            
        }
    }
}