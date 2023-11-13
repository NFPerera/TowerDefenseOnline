using System.Collections.Generic;
using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;
using _Main.Scripts.Networking;
using Clases;
using Enemies;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Models
{
    public class EnemyModel : SpawnableNetworkObject, IDamageable
    {
        [SerializeField] private EnemyData data;
        [SerializeField] private int index;
        [SerializeField]private SpriteRenderer sprite;
        
        private HealthController m_healthController;
        private List<Vector3> m_pathPoints = new List<Vector3>();
        private float m_speed;
        private int m_indexPathPoints;
        private int m_currLife;
        private bool m_isAlive;
        private void Awake()
        {
            foreach (var trans in GameManager.Instance.PathPoints)
            {
                m_pathPoints.Add(trans.position);
            }
            
            m_healthController = new HealthController(data.enemiesTierDatas[index].MaxHealth);

            sprite.sprite = data.enemiesTierDatas[index].Sprite;
            m_isAlive = true;
        }


        private void Update()
        {
            if(!m_isAlive)
                return;
            
            var position = transform.position;
            var distanceToTarget = Vector2.Distance(position, m_pathPoints[m_indexPathPoints < m_pathPoints.Count? m_indexPathPoints : 0]);
            var dir = Vector3.zero;

            if (distanceToTarget < 0.1f)
            {
                m_indexPathPoints++;
                
                if (m_indexPathPoints >= m_pathPoints.Count)
                {
                    MasterManager.Instance.RequestLooseLifePointsServerRpc(index+ 1);
                    MasterManager.Instance.RequestDespawnGameObjectServerRpc(MyOwnerId, NetworkObjectId);
                    return;
                }
                dir = (m_pathPoints[m_indexPathPoints] - position).normalized;
                
            }
            
            if(m_indexPathPoints < m_pathPoints.Count)
                dir = (m_pathPoints[m_indexPathPoints] - position).normalized;
            
            transform.Translate(dir * (data.enemiesTierDatas[index].Speed * Time.deltaTime));

        }

        #region IDamageable

            public Transform GetTransform() => transform;
            public ulong GetObjId() => NetworkObjectId;

            public void DoDamage(ulong attackerId, int damage)
            {
                m_healthController?.TakeDamage(damage);
                
                
                if (m_healthController?.Hp <= 0)  LowerTier(attackerId);
            }

            public void Heal(int healAmount) => m_healthController?.HealHp(healAmount);

        #endregion
        
        private void LowerTier(ulong attackerId)
        {
            index--;
            
            if (index < 0)
            {
                OnDie(attackerId);
                return;
            }
            MasterManager.Instance.RequestChangeMoneyServerRpc(attackerId, 5);
            ChangeStats();
        }
        
        private void ChangeStats()
        {
            m_healthController = new HealthController(data.enemiesTierDatas[index].MaxHealth);
            m_currLife = data.enemiesTierDatas[index].MaxHealth;
            sprite.sprite = data.enemiesTierDatas[index].Sprite;
        }
        private void OnDie(ulong attackerId)
        {
            m_isAlive = false;
            MasterManager.Instance.RequestChangeMoneyServerRpc(attackerId, 5);
            MasterManager.Instance.RequestDespawnGameObjectServerRpc(MyOwnerId, NetworkObjectId);
        }


        
    }
}