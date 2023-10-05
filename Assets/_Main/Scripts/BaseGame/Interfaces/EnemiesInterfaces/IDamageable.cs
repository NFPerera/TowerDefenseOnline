using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces
{
    public interface IDamageable : INetworkSerializable
    {
        Transform GetTransform();
        ulong GetObjId();
        void DoDamage(int damage);
        void Heal(int healAmount);
        
    }
}