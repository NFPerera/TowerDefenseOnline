using System;
using System.Collections.Generic;
using System.Linq;
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
        }
        
        
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
            if (m_instance != null && m_instance != this)
            {
                Destroy(this);
                return;
            }

            m_instance = this;
            
            m_serverId = NetworkManager.Singleton.LocalClientId;
            DontDestroyOnLoad(this.gameObject);

            m_lifePoints = MaxLifePoints;
            OnChangeLifePoints += OnLooseLifePoints;
        }

        public override void OnNetworkSpawn()
        {
            m_playersCount = 0;

            var a = FindObjectsByType<PlayerAvatarController>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            m_playersAvatars = a.ToList();
        }

        public void ChangeNetScene(string pSceneName) => NetworkManager.Singleton.SceneManager.LoadScene(pSceneName, LoadSceneMode.Single);

        [ServerRpc]
        public void SetPlayersNameServerRpc(ulong id, string pName)
        {
            if (m_playerDic.ContainsKey(id))
            {
                m_playerDic[id].PlayersName = pName;
            }
            else
            {
                m_playerDic[id] = new PlayerData();
                m_playerDic[id].PlayersName = pName;
                m_playerDic[id].Model?.SetPlayersName(pName);
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
            obj.transform.position = pos;
            obj.Spawn();

            if (obj.TryGetComponent(out SpawnableNetworkObject spawnableNetworkObject))
            {
                spawnableNetworkObject.SetOwnerId(p_OwnerId);
            }

            if (p_OwnerId == NetworkManager.Singleton.LocalClientId)
            {
                m_serverObj.Add(obj);
                return;
            }
            
            m_playerDic[p_OwnerId].Model.AddObjectToOwnerList(obj);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RequestDespawnGameObjectServerRpc(ulong ownerId,ulong networkObjId)
        {

            if (ownerId == m_serverId)
            {
                foreach (NetworkObject networkObject in m_serverObj)
                {
                    if(networkObject.NetworkObjectId != NetworkObjectId)
                        return;
                    
                    networkObject.Despawn();
                }
                return;
            }


            m_playerDic[ownerId].Model.TryGetOwnedObject(networkObjId, out var obj);
            obj.Despawn();
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestSpawnBulletServerRpc(ulong p_OwnerId, int spawnObjectId, Vector3 pos, ulong networkObjId)
        {
            var obj = Instantiate<NetworkObject>(SpawnableNetworkObjects[spawnObjectId]) ;
            obj.transform.position = pos;
            obj.Spawn();

            if (obj.TryGetComponent(out SpawnableNetworkObject spawnableNetworkObject))
            {
                spawnableNetworkObject.SetOwnerId(p_OwnerId);
            }

            m_playerDic[p_OwnerId].Model.AddObjectToOwnerList(obj);

            //TODO: Preguntarle a Seba como hacer esto mas performante
            //No me gusta que cada vez que se spawnee una bala tenga que buscar en toda la lista de objetos
            if (obj.TryGetComponent(out BulletModel model))
            {
                foreach (var networkObject in m_serverObj)
                {
                    if(networkObject.NetworkObjectId != NetworkObjectId)
                        return;

                    var targetTrans = networkObject.transform;
                    model.InitializeBullet(targetTrans);
                }
                
            }
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
        public void RequestDoDamageServerRpc(ulong objId, ulong attacker,int damage)
        {
            foreach (var obj in m_serverObj)
            {
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
                    Debug.Log($"Players in room: {m_playersCount}");
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
        
        #endregion

        #region Level
       
        
            [field: SerializeField] public int MaxLifePoints { get; private set; }
            
            private int m_lifePoints;
            
            private WaveController m_waveController;
            
            
            public Action<int> OnChangeLifePoints;
            public void SearchWaveController()
            {
                m_waveController = FindFirstObjectByType<WaveController>();
                m_waveController.OnFinishWave += RequestEnableWaveButtonsClientRpc;
            }
            
            [ServerRpc(RequireOwnership = false)]
            public void RequestActivateWaveServerRpc() 
            {
                Debug.Log($"WaveeStart");
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
                m_playerDic[affectedPlayer].Model.AddMoney(moneyChange);
            }
            
            private void OnLooseLifePoints(int lifeChange)
            {
                m_lifePoints -= lifeChange;

                if (m_lifePoints <= 0) LoseGame();
            }


            public int GetPlayersCurrMoney(ulong playersId) => m_playerDic[playersId].Model.PlayersMoney;
            public int GetLifePoints() => m_lifePoints;
            
            
            
            private void LoseGame()
            {
                //TODO; Cambiar la escena
            }

            #endregion
    }
}