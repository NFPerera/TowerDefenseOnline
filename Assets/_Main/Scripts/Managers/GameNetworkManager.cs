using System;
using System.Collections.Generic;
using _Main.Scripts.Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.Managers
{
    public class GameNetworkManager : NetworkBehaviour
    {
        [SerializeField] private List<NetworkObject> playersData;
        [SerializeField] private string victorySceneToLoad;
        [SerializeField] private string defeatSceneToLoad;
        
        private TMP_Text m_timerText;
        [SerializeField] private float timer = 120.0f;
        private const float SyncInterval = 3.0f;
        private float m_nextSyncTime = SyncInterval;
        private bool m_hasTimerStarted;

        
        public static GameNetworkManager Instance;
        // public static event Action OnGameSceneLoaded;
        public Dictionary<ulong, NetworkObject> ConnectedPlayerSpawnDatas = new Dictionary<ulong, NetworkObject>();
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // OnGameSceneLoaded += UpdateTimerTextReference;
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RequestAddPlayerSpawnDataServerRpc(int charId, ulong p_id)
        {
            var networkPrefab = playersData[charId];
            if (ConnectedPlayerSpawnDatas.TryAdd(p_id, networkPrefab))
                return;
            
            Debug.LogError("Error in Character Selection, Player already selected character");
            ConnectedPlayerSpawnDatas[p_id] = networkPrefab;
        }

        [ServerRpc]
        void SyncTimerServerRpc(float time)
        {
            timer = time;
            UpdateClientsTimerClientRpc(time);
        }

        [ClientRpc]
        void EndGameClientRpc(bool win)
        {
            if (win)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(victorySceneToLoad, LoadSceneMode.Single);
            }
            else
            {
                NetworkManager.Singleton.SceneManager.LoadScene(defeatSceneToLoad, LoadSceneMode.Single);
            }
        }

        [ClientRpc]
        void UpdateClientsTimerClientRpc(float time)
        {
            timer = time;
            UpdateTimerUI();
        }

        private void Update()
        {
            if (!m_hasTimerStarted)
                return;
            timer -= Time.deltaTime;

            if (IsServer)
            {
                m_nextSyncTime -= Time.deltaTime;

                if (m_nextSyncTime <= 0)
                {
                    SyncTimerServerRpc(timer);
                    m_nextSyncTime = SyncInterval;
                }
            }
            
            UpdateTimerUI();

            if (IsServer && timer <= 0)
            {
                //foreach (var player in GameManager.Instance.M_ConnectedModels)
                {
                    //EndGameClientRpc(!player.GetIsTag());
                }
            }
        }

        // private void OnDestroy()
        // {
        //     OnGameSceneLoaded -= UpdateTimerTextReference;
        // }

        public void UpdateTimerTextReference(TMP_Text timerText)
        {
            m_timerText = timerText;
            m_hasTimerStarted = true;
        }

        void UpdateTimerUI()
        {
            if (m_timerText != null)
            {
                m_timerText.text = timer.ToString("0");
            }
            else
            {
                Debug.LogWarning("TimerText is null. Unable to updateUI.");
            }
        }

        // public static void TriggerGameSceneLoaded()
        // {
        //     OnGameSceneLoaded?.Invoke();
        // }
    }
}