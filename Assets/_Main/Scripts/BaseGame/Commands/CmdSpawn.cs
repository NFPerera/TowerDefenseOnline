using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.BaseGame.Factory;
using _Main.Scripts.BaseGame.Interfaces;
using _Main.Scripts.BaseGame.Models;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Commands
{
    public class CmdSpawn : ICommando
    {
        private ulong m_requesterId;
        private SpawnableNetworkObject m_prefab;
        private SpawnableNetworkObject m_instance;
        private Vector3 m_position;

        private AFactory<SpawnableNetworkObject> m_gameObjectFactory = new AFactory<SpawnableNetworkObject>();

        //Pedir Id en el constructor
        public CmdSpawn(SpawnableNetworkObject prefab,ulong requesterId, Vector3 spawnPosition)
        {
            m_prefab = prefab;
            m_position = spawnPosition;
            m_requesterId = requesterId;
        }
        
        public void Execute()
        {
            MasterManager.Instance.RequestSpawnGameObjectServerRpc(m_requesterId, m_prefab.SpawnObjectId, m_position);
        }
        public void Undo()
        {
            if (m_instance.TryGetComponent(out TowerModel towerModel))
            {
                MasterManager.Instance.RequestChangeMoneyServerRpc(m_requesterId, +towerModel.GetData().Cost);
            }
            
            Object.Destroy(m_instance); 
        }
    }
}