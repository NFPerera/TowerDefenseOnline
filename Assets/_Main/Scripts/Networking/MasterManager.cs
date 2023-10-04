using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.Networking
{
    public class MasterManager : NetworkBehaviour
    {
        private class PlayerData
        {
            public string PlayersName;
            public PlayerModel Model;
        }
        [SerializeField] private NetworkObject playerPrefab;
        [SerializeField] private List<NetworkObject> SpawnableNetworkObjects = new List<NetworkObject>();
        public static MasterManager Instance => m_instance;
        private static MasterManager m_instance;

        private Dictionary<ulong, PlayerData> m_playerDic = new Dictionary<ulong, PlayerData>();

        private void Awake()
        {
            if (m_instance != null && m_instance != this)
            {
                Destroy(this);
                return;
            }

            m_instance = this;
        }

        [ServerRpc]
        public void SetPlayersNameServerRpc(ulong id, string p_name)
        {
            if (m_playerDic.ContainsKey(id))
            {
                m_playerDic[id].PlayersName = p_name;
            }
            else
            {
                m_playerDic[id] = new PlayerData();
                m_playerDic[id].PlayersName = p_name;
            }
        }

        //Esto deberia llamarse en el awake del model
        [ServerRpc(RequireOwnership = false)]
        public void RequestSpawnPlayerDicServerRpc(ulong id)
        {
            var obj = Instantiate<NetworkObject>(playerPrefab);
            obj.Spawn();
            
            
            if (m_playerDic.ContainsKey(id))
            {
                m_playerDic[id].Model = obj.GetComponent<PlayerModel>();
            }
            else
            {
                m_playerDic[id] = new PlayerData();
                m_playerDic[id].Model = obj.GetComponent<PlayerModel>();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestSpawnGameObjectServerRpc(ulong id, int spawnObjectId, Vector3 pos)
        {
            var obj = SpawnableNetworkObjects[spawnObjectId];
            obj.Spawn();
            obj.transform.position = pos;
            m_playerDic[id].Model.AddObjectToOwnerList(obj);
        }
    }
}