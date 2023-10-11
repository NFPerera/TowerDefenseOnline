using System;
using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.BaseGame.Commands;
using _Main.Scripts.DevelopmentUtilities.Extensions;
using _Main.Scripts.Networking;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace _Main.Scripts.BaseGame.Controllers
{
    public class WaveController : NetworkBehaviour
    {
        [System.Serializable]
        public class Wave
        {
            public SpawnableNetworkObject[] enemies;
            public int numberOfEnemies;
            public float countDownBetweenEnemies;
        }

        [SerializeField] private Wave[] waves;
        private int m_nextWave;
        private int m_spawnedEnemies;
        private float m_timer;
        private bool m_isWaveActive;
        private ulong m_serverId;
        private Vector2 m_spawnPoint = new Vector2(-10f,0.8f);
        
        private UIManager m_ui;
        public Action OnFinishWave;
        private void Start()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }

            m_nextWave = 0;
            m_serverId = OwnerClientId;
            m_ui = GetComponent<UIManager>();
        }

        private void Update()
        {
            if (m_nextWave < waves.Length)
            {
                if (m_nextWave == waves.Length - 1)
                {
                    SpawnFinalWave();
                }
                else if (m_isWaveActive)
                {
                    SpawnWave();
                }
                else
                {
                    m_timer = waves[m_nextWave].countDownBetweenEnemies;
                    m_spawnedEnemies = 0;
                    
                }
            }
            else
            {
                m_ui.ActivateGameOverScreen(true);
            }
        }

        private void SpawnWave()
        {
            m_timer -= Time.deltaTime;
            
            if (m_timer <= 0)
            {
                Random rnd = new Random();
                var aux = rnd.Next(0, waves[m_nextWave].enemies.Length);
                
                GameManager.Instance.AddEventQueue(new CmdSpawn(waves[m_nextWave].enemies[aux],m_serverId, m_spawnPoint.XY0()));
                m_timer = waves[m_nextWave].countDownBetweenEnemies;
                m_spawnedEnemies++;
            }

            if (m_spawnedEnemies > waves[m_nextWave].numberOfEnemies)
            {
                m_nextWave++;
                m_isWaveActive = false;
                OnFinishWave.Invoke();
            }
        }

        private void SpawnFinalWave()
        {
            m_timer -= Time.deltaTime;
            
            if (m_timer <= 0)
            {
                Random rnd = new Random();
                var aux = rnd.Next(0, 4);
                
                GameManager.Instance.AddEventQueue(new CmdSpawn(waves[m_nextWave].enemies[aux],m_serverId, m_spawnPoint));
                m_timer = waves[m_nextWave].countDownBetweenEnemies;
                m_spawnedEnemies++;
            }

            if (m_spawnedEnemies > waves[m_nextWave].numberOfEnemies)
            {
                m_nextWave++;
                m_isWaveActive = false;
            }
        }


        public void ActivateWave() => m_isWaveActive = true;


    }
}