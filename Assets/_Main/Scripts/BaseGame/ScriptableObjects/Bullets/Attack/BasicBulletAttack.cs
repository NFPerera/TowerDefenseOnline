using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.BaseGame.Commands;
using _Main.Scripts.BaseGame.Models;
using UnityEngine;

namespace _Main.Scripts.BaseGame.ScriptableObjects.Bullets.Attack
{
    [CreateAssetMenu(fileName = "BasicBulletAttack", menuName = "_main/Bullet/Data/Attack/BasicBulletAttack", order = 0)]
    public class BasicBulletAttack : BulletAttack
    {
        public override void Attack(BulletModel model)
        {
            GameManager.Instance.AddEventQueue(new CmdDoDamage(model.GetTargetID(), model.GetDamage()));
            
            Destroy(model.gameObject);
        }
    }
}