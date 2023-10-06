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
        private Collider2D[] m_overlapArea = new Collider2D[64];
        
        public override void Attack(BulletModel model)
        {
            if (model.GetTargetTransform() != null)
            {
                GameManager.Instance.AddEventQueue(new CmdDoDamage(model.GetTargetID(), model.GetDamage()));
                
                Physics2D.OverlapCircleNonAlloc(model.transform.position, explosionRadius, m_overlapArea);
                foreach (Collider2D collider in m_overlapArea)
                {
                    if(!collider.CompareTag("Enemies"))
                        return;

                    if (collider.TryGetComponent(out IDamageable damageable))
                    {
                        GameManager.Instance.AddEventQueue(new CmdDoDamage(damageable.GetObjId(), model.GetDamage()));
                    }
                }
            
                Destroy(model.gameObject);
            }
            else
                Destroy(model.gameObject);
            
        }
    }
}