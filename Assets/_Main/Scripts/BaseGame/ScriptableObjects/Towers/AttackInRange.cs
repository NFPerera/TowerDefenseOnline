using System.Collections.Generic;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;
using _Main.Scripts.BaseGame.Models;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.BaseGame.ScriptableObjects.Towers
{
    [CreateAssetMenu(fileName = "AttackInRange", menuName = "_main/Tower/Attack/Range", order = 0)]
    public class AttackInRange : TowerAttack
    {
        private List<EnemyModel> m_totalEnemiesInRange = new List<EnemyModel>();
        public override void Attack(TowerModel model)
        {
            m_totalEnemiesInRange = model.GetEnemiesInRange();
            
            if(m_totalEnemiesInRange == null || m_totalEnemiesInRange.Count <= 0) return;

            ChangeAimAngle(model);
            
            var firstEnemyInRange = m_totalEnemiesInRange[0];
            var data = model.GetData();
            
            //var bullet = Instantiate(data.BulletPrefabs, model.GetShootPoint().position, Quaternion.identity);
            var bul = data.BulletPrefabs;
            
            MasterManager.Instance.RequestSpawnBulletServerRpc(model.MyOwnerId, bul.SpawnObjectId, 
                model.GetShootPoint().position, firstEnemyInRange.NetworkObjectId);
            //bullet.InitializeBullet(firstEnemyInRange.GetTransform(), data.Damage);
        }


        private void ChangeAimAngle(TowerModel model)
        {
            Vector3 enemyPosition = m_totalEnemiesInRange[0].GetTransform().position;

            Vector3 aimDirection= (enemyPosition - model.transform.position).normalized;
            float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            model.transform.eulerAngles = new Vector3(0, 0, aimAngle);
        }
    }
}