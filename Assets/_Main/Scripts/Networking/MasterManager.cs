using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.Networking
{
    public class MasterManager : NetworkBehaviour
    {
        [SerializeField] private NetworkObject playerPrefab;
        [SerializeField] private readonly List<NetworkObject> SpawnableNetworkObjects = new List<NetworkObject>();
        public static MasterManager Instance => m_instance;
        private static MasterManager m_instance;

        private Dictionary<ulong, PlayerModel> m_playerDic = new Dictionary<ulong, PlayerModel>();

        private void Awake()
        {
            if (m_instance != null && m_instance != this)
            {
                Destroy(this);
                return;
            }

            m_instance = this;
        }

        //Esto deberia llamarse en el awake del model
        [ServerRpc(RequireOwnership = false)]
        public void RequestSpawnPlayerDicServerRpc(ulong id)
        {
            var obj = Instantiate<NetworkObject>(playerPrefab);
            obj.Spawn();
            m_playerDic[id] = obj.GetComponent<PlayerModel>();
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestSpawnGameObjectServerRpc(ulong id, int spawnObjectId, Vector3 pos)
        {
            var obj = SpawnableNetworkObjects[spawnObjectId];
            obj.Spawn();
            obj.transform.position = pos;
            m_playerDic[id].AddObjectToOwnerList(obj);
        }
    }
}