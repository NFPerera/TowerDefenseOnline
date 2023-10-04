using System;
using System.Collections.Generic;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts
{
    public class PlayerModel : NetworkBehaviour
    {
        private List<NetworkObject> m_ownedObjects = new List<NetworkObject>();

        private ulong m_myId;
        private void Start()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                this.enabled = false;
                return;
            }

            m_myId = NetworkManager.Singleton.LocalClientId;
            MasterManager.Instance.RequestSpawnPlayerDicServerRpc(m_myId);
        }

        public void AddObjectToOwnerList(NetworkObject o) => m_ownedObjects.Add(o);
        public void RemoveObjectToOwnerList(NetworkObject o) => m_ownedObjects.Remove(o);
    }
}