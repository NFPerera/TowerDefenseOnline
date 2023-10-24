using System.Collections.Generic;
using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.BaseGame.Commands;
using _Main.Scripts.BaseGame.Models;
using _Main.Scripts.DevelopmentUtilities;
using _Main.Scripts.DevelopmentUtilities.Extensions;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Main.Scripts
{
    public class PlayerModel : NetworkBehaviour
    {
        private List<NetworkObject> m_ownedObjects = new List<NetworkObject>();


        private int m_playersMoney;
        private ulong m_myId;

        public string PlayersName => m_playersName;
        private string m_playersName;

        private SpawnableNetworkObject m_towerToBuild;
        private UIManager m_myUiManager;
        private Camera m_mainCamera;
        private Vector2 m_mousePosition;
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
            m_mainCamera = Camera.main;
            InputManager.Instance.SubscribeInput("MousePos", OnMouseMovement);
            InputManager.Instance.SubscribeInput("LeftClick", OnLeftClick);
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

        private void OnMouseMovement(InputAction.CallbackContext obj)
        {
            m_mousePosition = obj.ReadValue<Vector2>();
        }
        
        private void OnLeftClick(InputAction.CallbackContext obj)
        {
            if(m_towerToBuild == null) return;
            Debug.Log($"right button pressed");
            var pos = m_mainCamera.ScreenToWorldPoint(m_mousePosition);
            CmdSpawn cmdSpawn = new CmdSpawn(m_towerToBuild,m_myId, pos.XY0());
            
            GameManager.Instance.AddEventQueue(cmdSpawn);
            GameManager.Instance.AddSellEvent(cmdSpawn);
            
            MasterManager.Instance.RequestChangeMoneyServerRpc(m_myId, -m_towerToBuild.GetComponent<TowerModel>().GetData().Cost);
            m_towerToBuild = null;
        }
        
        [ClientRpc]
        public void SetTowerToBuildClientRpc(int id)
        {
            
            m_towerToBuild = MasterManager.Instance.networkObjects[id].GetComponent<TowerModel>();
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