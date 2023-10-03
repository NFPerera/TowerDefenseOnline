using System;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.Networking
{
    public class PlayerNetAnimator : NetworkBehaviour
    {
        public Animator Animator;
        

        private void Start()
        {
            if(!IsOwner) enabled = false;
        }

        private void Update()
        {
            throw new NotImplementedException();
        }
    }
}