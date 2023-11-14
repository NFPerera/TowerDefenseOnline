﻿using System;
using System.Collections.Generic;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;
using _Main.Scripts.BaseGame.Interfaces.TowerInterfaces;
using _Main.Scripts.BaseGame.ScriptableObjects.Towers;
using _Main.Scripts.Networking;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Models
{
    public class TowerModel : SpawnableNetworkObject, ITower
    {
        [SerializeField] private TowerData data;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private ParticleSystem attackParticleSystem;
        public GameObject towerBody;
        private List<EnemyModel> m_enemiesInRange = new List<EnemyModel>();
        private float m_timer;

        private void Awake()
        {
            
                
        }

        private void Update()
        {
            if(!IsServer)
                return;
            
            m_timer += Time.deltaTime;
            
            if(m_enemiesInRange.Count <= 0f)
                return;
            
            if (m_timer >= data.AttackSpeed)
            {
                Attack();
                m_timer = 0f;
            }
        }
        public void Attack() => data.TowerAttack.Attack(this);

        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if(!IsServer)
                return;
            if (!col.TryGetComponent(out EnemyModel damageable)) return;
            
            m_enemiesInRange.Add(damageable);
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if(!IsServer)
                return;
            if (!other.TryGetComponent(out EnemyModel damageable)) return;
            
            m_enemiesInRange.Remove(damageable);
        }

        public void PlayAttackParticleSystem() => attackParticleSystem.Play();
        #region Getters
            public TowerData GetData() => data;
            public List<EnemyModel> GetEnemiesInRange() => m_enemiesInRange;
            public Transform GetShootPoint() => shootPoint;

        #endregion
    }
}