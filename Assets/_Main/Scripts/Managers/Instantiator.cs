using System;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class Instantiator : MonoBehaviour
    {
        private void Start()
        {
            if(!NetworkManager.Singleton.IsServer)
                MasterManager.Instance.RequestSpawnPlayerDicServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }
}