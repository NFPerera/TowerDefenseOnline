using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.BaseGame.Commands;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace _Main.Scripts.BaseGame.Controllers
{
    public class WaveController : MonoBehaviour
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
        
        [SerializeField] private Transform spawnPoint;


        private UIManager m_ui;
        private void Awake()
        {
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
                var aux = rnd.Next(0, 3);
                
                GameManager.Instance.AddEventQueue(new CmdSpawn(waves[m_nextWave].enemies[aux], spawnPoint.position));
                m_timer = waves[m_nextWave].countDownBetweenEnemies;
                m_spawnedEnemies++;
            }

            if (m_spawnedEnemies > waves[m_nextWave].numberOfEnemies)
            {
                m_nextWave++;
                m_isWaveActive = false;
            }
        }

        private void SpawnFinalWave()
        {
            m_timer -= Time.deltaTime;
            
            if (m_timer <= 0)
            {
                Random rnd = new Random();
                var aux = rnd.Next(0, 4);
                
                GameManager.Instance.AddEventQueue(new CmdSpawn(waves[m_nextWave].enemies[aux], spawnPoint.position));
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