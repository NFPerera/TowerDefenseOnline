using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces
{
    public interface IDamageable
    {
        Transform GetTransform();
        ulong GetObjId();
        void DoDamage(ulong attackerId, int damage);
        void Heal(int healAmount);
        
    }
}