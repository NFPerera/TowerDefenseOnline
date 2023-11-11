using System;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Clases
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
        private void Update()
        {
            transform.eulerAngles += new Vector3(0, 0, speed) * Time.deltaTime;
        }
    }
}