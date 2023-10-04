using _Main.Scripts.BaseGame.Commands;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.BaseGame._Managers
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager Instance;

        private SpawnableNetworkObject m_towerToBuild;
        private MousePosition m_mousePosition;
        private void Awake()
        {
            if(Instance != null) Destroy(this);
                Instance = this;

            m_mousePosition = GameObject.FindGameObjectWithTag("Mouse").GetComponent<MousePosition>();
            GameManager.Instance.OnClick += BuildTowerInWorld;
        }
        public void SetTowerToBuild(SpawnableNetworkObject towerToBuild) => m_towerToBuild = towerToBuild;

        private void BuildTowerInWorld()
        {
            var clickPosition = m_mousePosition.GetMousePosition();
            if(m_towerToBuild == null) return;
            
            CmdSpawn cmdSpawn = new CmdSpawn(m_towerToBuild, clickPosition);
            
            GameManager.Instance.AddEventQueue(cmdSpawn);
            GameManager.Instance.AddSellEvent(cmdSpawn);
            
            m_towerToBuild = null;
        }
    }
}