using _Main.Scripts.BaseGame.Clases;
using _Main.Scripts.BaseGame.Commands;
using _Main.Scripts.BaseGame.Interfaces.BulletsInterfaces;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;
using _Main.Scripts.BaseGame.ScriptableObjects.Bullets;
using _Main.Scripts.Networking;
using Clases;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Models
{
    [RequireComponent(typeof(MovementController))]
    public class BulletModel : SpawnableNetworkObject, IBullet
    {
        [SerializeField] private BulletData data;

        private Transform m_target;
        private IDamageable m_targetDamageable;
        private ulong m_targetDamageableId;
        private bool m_reachTarget;
        private bool m_isActive;
        private ulong m_objId;

        private Vector3 m_dir;
        public BulletData GetData() => data;
        public void InitializeBullet(Transform target)
        {
            m_target = target;
            m_reachTarget = false;
            m_isActive = true;

            m_dir = (m_target.position - transform.position).normalized;
            m_objId = NetworkObjectId;
        }
        private void Update()
        {
            if (!m_reachTarget && m_target != null)
            {
                transform.Translate(m_dir * (data.Speed * Time.deltaTime));
            }
            else if (m_isActive)
            {
                MasterManager.Instance.RequestDespawnGameObjectServerRpc(MyOwnerId, m_objId);
                m_isActive = false;
                gameObject.SetActive(false);
            }
            
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(!IsServer)
                return;
            
            if (!col.TryGetComponent(out IDamageable damageable)) return;

            if (!m_reachTarget)
            {
                SetTargetDamageable(damageable);
                SetTargetDamageableId(col.GetComponent<NetworkObject>().NetworkObjectId);
                data.BulletAttack.Attack(this);
                m_reachTarget = true;
            }
        }

        private void SetTargetDamageable(IDamageable target) => m_targetDamageable = target;
        private void SetTargetDamageableId(ulong targetId) => m_targetDamageableId = targetId;

        #region Getters
            public IDamageable GetTargetIDamageable() => m_targetDamageable;
            public ulong GetTargetID() => m_targetDamageableId;
            public Transform GetTargetTransform() => m_target;
            public int GetDamage() => data.Damage;
            public ulong GetNetworkId() => m_objId;

        #endregion
        

        
    }
}