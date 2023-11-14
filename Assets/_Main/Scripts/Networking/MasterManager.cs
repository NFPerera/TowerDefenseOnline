using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.BaseGame.Controllers;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;
using _Main.Scripts.BaseGame.Models;
using _Main.Scripts.Menus;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.Networking
{
    public class MasterManager : NetworkBehaviour
    {
        private class PlayerData
        {
            public string PlayersName;
            public PlayerModel Model;
            public List<NetworkObject> PlayersObj = new List<NetworkObject>();
        }
        
        
        [SerializeField] private NetworkObject playerPrefab;
        [SerializeField] private List<NetworkObject> SpawnableNetworkObjects = new List<NetworkObject>();
        public List<NetworkObject> networkObjects = new List<NetworkObject>();
        public static MasterManager Instance => m_instance;
        private static MasterManager m_instance;

        private Dictionary<ulong, PlayerData> m_playerDic = new Dictionary<ulong, PlayerData>();
        private List<NetworkObject> m_serverObj = new List<NetworkObject>();
        
        private ulong m_serverId;
        private const int MaxUndos = 25;
        private void Awake()
        {
            if (m_instance != null && m_instance != this)
            {
                Destroy(this);
                return;
            }

            m_instance = this;
            
            m_serverId = NetworkManager.Singleton.LocalClientId;
            DontDestroyOnLoad(this.gameObject);

            m_lifePoints = MaxLifePoints;
            networkObjects = SpawnableNetworkObjects;
        }

        public override void OnNetworkSpawn()
        {
            m_playersCount = 0;

            var a = FindObjectsByType<PlayerAvatarController>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            m_playersAvatars = a.ToList();
        }

        public void ChangeNetScene(string pSceneName) => NetworkManager.Singleton.SceneManager.LoadScene(pSceneName, LoadSceneMode.Single);


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
                var data = new PlayerData();
                data.Model = obj.GetComponent<PlayerModel>();
                m_playerDic.Add(id, data);
                RequestChangeMoneyServerRpc(id, 1000);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestSpawnGameObjectServerRpc(ulong p_OwnerId, int spawnObjectId, Vector3 pos)
        {
            var obj = Instantiate<NetworkObject>(SpawnableNetworkObjects[spawnObjectId]) ;
            
            
            if (!obj.TryGetComponent(out SpawnableNetworkObject spawnableNetworkObject))
                return;
            
            obj.transform.position = pos;
            obj.Spawn();
            spawnableNetworkObject.SetOwnerIdClientRpc(p_OwnerId);
            
            if (p_OwnerId == NetworkManager.Singleton.LocalClientId)
            {
                m_serverObj.Add(obj);
            }
            else
            {
                m_playerDic[p_OwnerId].PlayersObj.Add(obj);
                m_playerDic[p_OwnerId].Model.AddObjectToOwnerList(obj.NetworkObjectId);
            }
        }
        
        public void RequestDespawnGameObject(ulong ownerId,ulong networkObjId)
        {
            if (ownerId == m_serverId)
            {
                for (int i = 0; i < m_serverObj.Count; i++)
                {
                    var networkObject = m_serverObj[i];
                    
                    if(networkObject.NetworkObjectId != networkObjId)
                        continue;

                    networkObject.Despawn();
                    m_serverObj.Remove(networkObject);
                    
                }
                return;
            }


            m_playerDic[ownerId].Model.TryGetOwnedObject(networkObjId, out var objId);

            foreach (var obj in m_playerDic[ownerId].PlayersObj)
            {
                if ((obj.NetworkObjectId == objId))
                {
                    m_playerDic[ownerId].PlayersObj.Remove(obj);
                    m_playerDic[ownerId].Model.RemoveObjectToOwnerList(obj.NetworkObjectId);
                    obj.Despawn();
                    break;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestSpawnBulletServerRpc(ulong p_OwnerId, int spawnObjectId, Vector3 pos, ulong p_targetNetworkObjId)
        {
            var obj = Instantiate<NetworkObject>(SpawnableNetworkObjects[spawnObjectId]) ;
            obj.transform.position = pos;
            obj.Spawn();

            if (obj.TryGetComponent(out SpawnableNetworkObject spawnableNetworkObject))
            {
                spawnableNetworkObject.SetOwnerIdClientRpc(p_OwnerId);
            }
            
            m_playerDic[p_OwnerId].PlayersObj.Add(obj);
            m_playerDic[p_OwnerId].Model.AddObjectToOwnerList(obj.NetworkObjectId);

            if (obj.TryGetComponent(out BulletModel model))
            {
                foreach (var enemy in m_serverObj)
                {
                    if(enemy.NetworkObjectId != p_targetNetworkObjId)
                        continue;

                    var targetTrans = enemy.transform;
                    model.InitializeBullet(targetTrans);
                    break;
                }
            }
        }


        [ServerRpc(RequireOwnership = false)]
        public void RequestDisconnectUserServerRpc(ulong id)
        {
            if (id == NetworkManager.Singleton.LocalClientId)
            {
                NetworkManager.Singleton.Shutdown();
            }
            else
            {
                Destroy(m_playerDic[id].Model);
                m_playerDic.Remove(id);
                NetworkManager.Singleton.DisconnectClient(id);
            }
            
            
        }

        
        
        [ServerRpc(RequireOwnership = false)]
        public void RequestDoDamageServerRpc(ulong objId, ulong attacker,int damage)
        {
            for (int i = 0; i < m_serverObj.Count; i++)
            {
                var obj = m_serverObj[i];
                
                if(obj == default)
                    continue;
                
                if(obj.NetworkObjectId != objId)
                    continue;
                
                if (obj.TryGetComponent(out IDamageable damageable))
                {
                    damageable.DoDamage(attacker, damage);
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

        #region Main Menu

            [System.Serializable]
            public class RoomData
            {
                public int RoomId;
                public string Name;
            }
            
            private List<PlayerAvatarController> m_playersAvatars = new List<PlayerAvatarController>();
            private Dictionary<ulong, RoomData> m_roomDatas = new Dictionary<ulong, RoomData>();
            private int m_playersCount;
            
            
            [ServerRpc(RequireOwnership = false)]
            public void RequestPlayerJoinRoomUpdateServerRpc(ulong p_ulong, string p_playersName)
            {
                if(p_ulong == m_serverId)
                    return;
                
                
                var data = new RoomData();
                
                
                data.Name = p_playersName;
                data.RoomId = m_playersCount;
                
                
                if (m_roomDatas.TryAdd(p_ulong, data))
                {
                    var json = Serializador.SerializeDic(m_roomDatas);
                    RefreshDictionaryClientRpc(json);
                    m_playersCount++;
                }
                else
                {
                    Debug.LogError("Player ID was already in the Dictionary");
                } 
            }

            private void RefreshWaitingRoomView()
            {
                foreach (var data in m_roomDatas)
                {
                    var roomId = data.Value.RoomId;
                    var avatar = m_playersAvatars[roomId];
                    avatar.SetPlayersName(data.Value.Name);
                    avatar.Activate();
                }
            }

            [ClientRpc]
            private void RefreshDictionaryClientRpc(string json)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    RefreshWaitingRoomView();
                    return;
                }
                
                Serializador.DeSerializeDic(json, m_roomDatas);
                RefreshWaitingRoomView();
            }

            public RoomData GetRoomData(ulong id) => m_roomDatas[id];
        
        #endregion

        #region Level
       
        
            [field: SerializeField] public int MaxLifePoints { get; private set; }
            [SerializeField] private String victorySceneName;
            [SerializeField] private String defeatSceneName;
            
            private int m_lifePoints;
            
            private WaveController m_waveController;
            
            //public Action<int> OnChangeLifePoints;
            public void SetWaveController(WaveController waveController)
            {
                m_waveController = waveController;
                m_waveController.OnFinishWave += RequestEnableWaveButtonsClientRpc;
            }
            
            [ServerRpc(RequireOwnership = false)]
            public void RequestActivateWaveServerRpc() 
            {
                m_waveController.ActivateWave();
                RequestUnableWaveButtonsClientRpc();
            }
        
            [ClientRpc]
            public void RequestUnableWaveButtonsClientRpc()
            {
                GameManager.Instance.ToggleWaveButton(false);
            }
            
            [ClientRpc]
            public void RequestEnableWaveButtonsClientRpc()
            {
                GameManager.Instance.ToggleWaveButton(true);
            }

            [ServerRpc(RequireOwnership = false)]
            public void RequestChangeMoneyServerRpc(ulong affectedPlayer, int moneyChange)
            {
                var p = new ClientRpcParams();
                p.Send.TargetClientIds = new ulong[]{affectedPlayer,m_serverId};
                var model = m_playerDic[affectedPlayer].Model;

                var diff = model.GetMoney() + moneyChange;
                m_playerDic[affectedPlayer].Model.RequestChangeMoneyClientRpc(diff,p);
            }
            
            public void RequestLooseLifePoints(int lifeChange)
            {
                m_lifePoints -= lifeChange;
                LooseLifePointsClientRpc(lifeChange);
                
                if (m_lifePoints <= 0)
                    NetworkManager.Singleton.SceneManager.LoadScene(defeatSceneName, LoadSceneMode.Single);
            }

            public Action<int> OnChangeLifePoints;

            [ClientRpc]
            private void LooseLifePointsClientRpc(int lifeChange)
            {
                if(IsServer)
                    return;
                
                m_lifePoints -= lifeChange;
                OnChangeLifePoints.Invoke(m_lifePoints);
            }

            [ServerRpc]
            public void RequestLoadWinSceneServerRpc()
            {
                NetworkManager.Singleton.SceneManager.LoadScene(victorySceneName, LoadSceneMode.Single);
            }
            [ServerRpc(RequireOwnership = false)]
            public void RequestBuyTowerServerRpc(ulong buyerId,int towerId, int towerCost)
            {
                if (m_playerDic[buyerId].Model.GetMoney() > towerCost)
                {
                    m_playerDic[buyerId].Model.SetTowerToBuildClientRpc(towerId);
                }
            }

             
            public int GetPlayersCurrMoney(ulong playersId) => m_playerDic[playersId].Model.GetMoney();
            public int GetLifePoints() => m_lifePoints;
            
            

            #endregion
    }
}
