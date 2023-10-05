using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using _Main.Scripts.BaseGame.Interfaces;
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
        
        private List<ICommando> m_events = new List<ICommando>();
        private Stack<ICommando> m_sellableEvents = new Stack<ICommando>();
        private List<ICommando> m_doneEvents = new List<ICommando>();
        
        private const int MaxUndos = 25;
        private int m_lifePoints;
        private int m_money;
        
        
        public Action<int> OnChangeLifePoints;
        public Action<int> OnChangeMoney;
        public Action OnClick;
        public Action OnGameOver;
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
            
            m_lifePoints = MaxLifePoints;
            m_money = StartMoneyPoints;
            
            
            OnChangeLifePoints += OnLooseLifePointsServerRpc;
            OnChangeMoney += OnChangeMoneyServerRpc;
            OnGameOver += LoseGame;
        }
        
        private void Update()
        {
            foreach (var events in m_events)
            {
                events.Execute();
                m_doneEvents.Add(events);

                if (m_doneEvents.Count > MaxUndos)
                    m_doneEvents.RemoveAt(0);
            }

            m_events.Clear();
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
        public void RequestSpawnGameObjectServerRpc(ulong id, int spawnObjectId, Vector3 pos)
        {
            var obj = SpawnableNetworkObjects[spawnObjectId];
            obj.Spawn();
            obj.transform.position = pos;
            m_playerDic[id].Model.AddObjectToOwnerList(obj);
        }
        
    #region Facade

        [ServerRpc(RequireOwnership = false)]
        public void AddEventQueueServerRpc(ICommando commando) => m_events.Add(commando);
        [ServerRpc(RequireOwnership = false)]
        public void AddSellEventServerRpc(ICommando commando) => m_sellableEvents.Push(commando);

    #endregion
        
    #region Memento
        
        [ServerRpc(RequireOwnership = false)]
        public void SellLastTowerServerRpc() => m_sellableEvents.Pop().Undo();
        [ServerRpc(RequireOwnership = false)]
        public void UndoAllEventListServerRpc()
        {
            if(m_doneEvents.Count <= 0) return;

            for (int i = 0; i < m_doneEvents.Count; i++)
            {
                m_doneEvents[i].Undo();
                m_doneEvents.RemoveAt(m_doneEvents.Count - 1);
            }
        }

    #endregion
    
    #region Getters

        public int GetLifePoints() => m_lifePoints;
        public int GetMoney() => m_money;

    #endregion
    
    #region GAME RULES;

        [ServerRpc(RequireOwnership = false)]
        private void OnLooseLifePointsServerRpc(int lifeChange)
        {
            m_lifePoints -= lifeChange;

            if (m_lifePoints <= 0) LoseGame();
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnChangeMoneyServerRpc(int moneyChange)
        {
            m_money += moneyChange;
        }

        
        private void LoseGame()
        {
        }

    #endregion
    }
}