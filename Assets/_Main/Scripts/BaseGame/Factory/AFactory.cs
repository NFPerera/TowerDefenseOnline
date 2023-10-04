using _Main.Scripts.BaseGame.Interfaces.FactoriesInterfaces;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Factory
{
    public class AFactory<T> : IFactory<T> where T : Object
    {
        public T Create(T prefab) => Object.Instantiate(prefab);
    }
}