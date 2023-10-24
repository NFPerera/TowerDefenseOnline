using System.Collections.Generic;
using _Main.Scripts.BaseGame._Managers;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts
{
    public class PlayerModel : NetworkBehaviour
    {
        private List<NetworkObject> m_ownedObjects = new List<NetworkObject>();


        private int m_playersMoney;
        private ulong m_myId;

        public string PlayersName => m_playersName;
        private string m_playersName;

        private UIManager m_myUiManager;

        public override void OnNetworkSpawn()
        {
            m_myUiManager = FindFirstObjectByType<UIManager>();
        }

        private void Start()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                this.enabled = false;
                return;
            }

            m_myId = NetworkManager.Singleton.LocalClientId;
            
            
            Debug.Log($"Start Playermodel, ui: {m_myUiManager != null}");
        }

        public void SetPlayersName(string p_s) => m_playersName = p_s;

        public void AddObjectToOwnerList(NetworkObject o) => m_ownedObjects.Add(o);
        public void RemoveObjectToOwnerList(NetworkObject o) => m_ownedObjects.Remove(o);

        public bool TryGetOwnedObject(ulong p_objId, out NetworkObject networkObject)
        {
            foreach (var obj in m_ownedObjects)
            {
                if (p_objId == obj.NetworkObjectId)
                {
                    networkObject = obj;
                    return true;
                }
            }

            networkObject = default;
            return false;
        }

        

        [ClientRpc]
        public void RequestChangeMoneyClientRpc(int money, ClientRpcParams p)
        {
            m_playersMoney = money;
            
            if(!IsServer)
                m_myUiManager.UpdateMoneyText(m_playersMoney);
        }
        public int GetMoney() => m_playersMoney;

    }
}