using _Main.Scripts.BaseGame.Models;
using UnityEngine;

namespace _Main.Scripts.BaseGame.ScriptableObjects.Bullets.Movement
{
    public abstract class BulletMovement : ScriptableObject
    {
        public abstract void Move(BulletModel model);
    }
}