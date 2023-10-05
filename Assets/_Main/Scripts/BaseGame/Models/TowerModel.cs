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

        private List<IDamageable> _enemiesInRange = new List<IDamageable>();
        private float _timer;
        private void Update()
        {
            _timer += Time.deltaTime;
            
            if (_timer >= data.AttackSpeed)
            {
                Attack();
                _timer = 0f;
            }
        }
        public void Attack() => data.TowerAttack.Attack(this);

        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.TryGetComponent(out IDamageable damageable)) return;
            
            _enemiesInRange.Add(damageable);
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent(out IDamageable damageable)) return;
            
            _enemiesInRange.Remove(damageable);
        }

        #region Getters
            public TowerData GetData() => data;
            public List<IDamageable> GetEnemiesInRange() => _enemiesInRange;
            public Transform GetShootPoint() => shootPoint;

        #endregion
    }
}