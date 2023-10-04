using _Main.Scripts.BaseGame.Models;
using UnityEngine;

namespace _Main.Scripts.BaseGame.ScriptableObjects.Bullets.Attack
{
    public abstract class BulletAttack : ScriptableObject
    {
        public abstract void Attack(BulletModel model);
    }
}