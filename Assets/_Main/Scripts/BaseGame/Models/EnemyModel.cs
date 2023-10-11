using System.Collections.Generic;
using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.BaseGame.Clases;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;
using _Main.Scripts.Networking;
using Clases;
using Enemies;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Models
{
    [RequireComponent(typeof(MovementController))]
    public class EnemyModel : SpawnableNetworkObject, IDamageable
    {
        [SerializeField] private EnemyData data;
        [SerializeField] private int index;
        
        
        private HealthController m_healthController;
        private SpriteRenderer m_sprite;
        private List<Vector3> m_pathPoints = new List<Vector3>();
        private float m_speed;
        private int m_indexPathPoints;
        private int m_currLife;
        private void Awake()
        {
            foreach (var trans in GameManager.Instance.PathPoints)
            {
                m_pathPoints.Add(trans.position);
            }
            
            m_healthController = new HealthController(data.enemiesTierDatas[index].MaxHealth);

            m_sprite = gameObject.GetComponent<SpriteRenderer>();
            m_sprite.sprite = data.enemiesTierDatas[index].Sprite;
        }


        private void Update()
        {
            var position = transform.position;
            var distanceToTarget = Vector2.Distance(position, m_pathPoints[m_indexPathPoints < m_pathPoints.Count? m_indexPathPoints : 0]);
            var dir = Vector3.zero;

            if (distanceToTarget < 0.1f)
            {
                m_indexPathPoints++;
                
                if (m_indexPathPoints >= m_pathPoints.Count)
                {
                    GameManager.Instance.OnChangeLifePoints.Invoke(index+ 1);
                    Destroy(gameObject);
                    
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

            public void DoDamage(int damage)
            {
                m_healthController?.TakeDamage(damage);
                
                if (m_healthController?.Hp <= 0)  LowerTier();
            }

            public void Heal(int healAmount) => m_healthController?.HealHp(healAmount);

        #endregion
        
        private void LowerTier()
        {
            index--;
            
            if (index < 0)
            {
                OnDie();
                return;
            }
            GameManager.Instance.OnChangeMoney.Invoke(5);
            ChangeStats();
        }
        
        private void ChangeStats()
        {
            m_healthController = new HealthController(data.enemiesTierDatas[index].MaxHealth);
            m_currLife = data.enemiesTierDatas[index].MaxHealth;
            m_sprite.sprite = data.enemiesTierDatas[index].Sprite;
        }
        private void OnDie()
        {
            GameManager.Instance.OnChangeMoney.Invoke(10);
            RequestDestroyObjServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestDestroyObjServerRpc()
        {
            //PORQUEEEEEEE ME TIRA QUE NO DEBERRIA DESTRUIRLO?????????
            Destroy(this.gameObject);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref m_currLife);
        }
    }
}