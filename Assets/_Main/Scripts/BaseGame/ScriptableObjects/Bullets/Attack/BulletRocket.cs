using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.BaseGame.Commands;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;
using _Main.Scripts.BaseGame.Models;
using UnityEngine;

namespace _Main.Scripts.BaseGame.ScriptableObjects.Bullets.Attack
{
    [CreateAssetMenu(fileName = "BulletRocket", menuName = "_main/Bullet/Data/Attack/RocketBulletAttack", order = 0)]
    public class BulletRocket : BulletAttack
    {
        [SerializeField] private float explosionRadius = 3f;
        [SerializeField] private LayerMask enemiesMask;
        private Collider2D[] m_overlapArea = new Collider2D[64];
        
        public override void Attack(BulletModel model)
        {
            if (model.GetTargetTransform() != null)
            {
                GameManager.Instance.AddEventQueue(new CmdDoDamage(model.GetTargetID(), model.MyOwnerId,model.GetDamage()));
                
                var count = Physics2D.OverlapCircleNonAlloc(model.transform.position, explosionRadius, m_overlapArea);

                for (int i = 0; i < count; i++)
                {
                    var collider = m_overlapArea[i];
                    
                    if(collider.gameObject.layer != enemiesMask)
                        continue;

                    if (collider.TryGetComponent(out IDamageable damageable))
                    {
                        GameManager.Instance.AddEventQueue(new CmdDoDamage(damageable.GetObjId(),model.MyOwnerId, model.GetDamage()));
                    }
                }
            }
            
        }
    }
}