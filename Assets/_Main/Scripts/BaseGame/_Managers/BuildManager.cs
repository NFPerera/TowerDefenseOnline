using _Main.Scripts.BaseGame.Commands;
using _Main.Scripts.DevelopmentUtilities;
using _Main.Scripts.DevelopmentUtilities.Extensions;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Main.Scripts.BaseGame._Managers
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager Instance;

        private SpawnableNetworkObject m_towerToBuild;
        private Camera m_mainCamera;
        private Vector2 m_mousePosition;
        private ulong m_id;
        private void Start()
        {
            // if (NetworkManager.Singleton.IsServer)
            // {
            //     this.enabled = false;
            //     return;
            // }
            
            if(Instance != null) Destroy(this);
                Instance = this;

                //TODO: Refactorizar todo lo del mouse con el nuevo input sisteem
            //m_mousePosition = GameObject.FindGameObjectWithTag("Mouse").GetComponent<MousePosition>();
            
            InputManager.Instance.SubscribeInput("MousePos", OnMouseMovement);
            InputManager.Instance.SubscribeInput("LeftClick", OnLeftClick);
            m_id = NetworkManager.Singleton.LocalClientId;
            m_mainCamera = Camera.main;
            //GameManager.Instance.OnClick += BuildTowerInWorld;
        }

        private void OnLeftClick(InputAction.CallbackContext obj)
        {
            if(m_towerToBuild == null) return;
            
            var pos = m_mainCamera.ScreenToWorldPoint(m_mousePosition);
            CmdSpawn cmdSpawn = new CmdSpawn(m_towerToBuild,m_id, pos.XY0());
            
            GameManager.Instance.AddEventQueue(cmdSpawn);
            GameManager.Instance.AddSellEvent(cmdSpawn);
            
            m_towerToBuild = null;
        }

        private void OnMouseMovement(InputAction.CallbackContext obj)
        {
            m_mousePosition = obj.ReadValue<Vector2>();
        }

        public void SetTowerToBuild(SpawnableNetworkObject towerToBuild) => m_towerToBuild = towerToBuild;

        // private void BuildTowerInWorld()
        // {
        //     var clickPosition = m_mousePosition.GetMousePosition();
        //     if(m_towerToBuild == null) return;
        //     
        //     CmdSpawn cmdSpawn = new CmdSpawn(m_towerToBuild, clickPosition);
        //     
        //     GameManager.Instance.AddEventQueue(cmdSpawn);
        //     GameManager.Instance.AddSellEvent(cmdSpawn);
        //     
        //     m_towerToBuild = null;
        // }
    }
}