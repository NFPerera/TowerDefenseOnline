using System.Collections.Generic;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;
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
        
        [field: SerializeField] public int MaxLifePoints { get; private set; }
        [field: SerializeField] public int StartMoneyPoints { get; private set; }
        [SerializeField] private NetworkObject playerPrefab;
        [SerializeField] private List<NetworkObject> SpawnableNetworkObjects = new List<NetworkObject>();
        
        public static MasterManager Instance => m_instance;
        private static MasterManager m_instance;

        private Dictionary<ulong, PlayerData> m_playerDic = new Dictionary<ulong, PlayerData>();
        private List<NetworkObject> m_serverObj = new List<NetworkObject>();
        
        private ulong m_serverId;
        private const int MaxUndos = 25;
        private void Awake()
        {
            if (!IsServer)
            {
                this.enabled = false;
                return;
            }
            if (m_instance != null && m_instance != this)
            {
                Destroy(this);
                return;
            }

            m_instance = this;
            //Este es el id del server?????
            m_serverId = NetworkManager.Singleton.LocalClientId;

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
                m_playerDic[id].Model?.SetPlayersName(p_name);
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
        public void RequestSpawnGameObjectServerRpc(ulong p_OwnerId, int spawnObjectId, Vector3 pos)
        {
            var obj = Instantiate<NetworkObject>(SpawnableNetworkObjects[spawnObjectId]) ;
            obj.Spawn();
            obj.transform.position = pos;

            if (obj.TryGetComponent(out SpawnableNetworkObject spawnableNetworkObject))
            {
                spawnableNetworkObject.SetOwnerId(p_OwnerId);
            }

            if (p_OwnerId == m_serverId)
            {
                m_serverObj.Add(obj);
                return;
            }
            
            m_playerDic[p_OwnerId].Model.AddObjectToOwnerList(obj);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestMoveCommandServerRpc(ulong p_ownerId, ulong p_objId, Vector3 p_dir, float p_speed)
        {
            var model = m_playerDic[p_ownerId].Model;
            if (model.TryGetOwnedObject(p_objId, out var networkObject))
            {
                networkObject.transform.position += p_dir * (p_speed * Time.deltaTime);
            }
            
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestDoDamageServerRpc(ulong p_objId, int damage)
        {
            foreach (var obj in m_serverObj)
            {
                if(obj.NetworkObjectId != p_objId)
                    continue;
                
                if (obj.TryGetComponent(out IDamageable damageable))
                {
                    damageable.DoDamage(damage);
                }
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RequestDoHealServerRpc(ulong p_objId, int damage)
        {
            foreach (var obj in m_serverObj)
            {
                if(obj.NetworkObjectId != p_objId)
                    continue;
                
                if (obj.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Heal(damage);
                }
            }
        }
    }
}