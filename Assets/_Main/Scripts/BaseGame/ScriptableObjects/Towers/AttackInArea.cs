using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.BaseGame.Commands;
using _Main.Scripts.BaseGame.Models;
using UnityEngine;

namespace Towers
{
    [CreateAssetMenu(fileName = "AttackInArea", menuName = "_main/Tower/Attack/Area", order = 0)]
    public class AttackInArea : TowerAttack
    {
        public override void Attack(TowerModel model)
        {
            var enemiesInRange = model.GetEnemiesInRange();
            var data = model.GetData();
            
            for (int i = 0; i < enemiesInRange.Count; i++)
            {
                GameManager.Instance.AddEventQueue(new CmdDoDamage(enemiesInRange[i], data.Damage));
            }
        }
    }
}