using UnityEngine;

namespace _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces
{
    public interface IDamageable
    {
        Transform GetTransform();
        void DoDamage(int damage);
        void Heal(int healAmount);
    }
}