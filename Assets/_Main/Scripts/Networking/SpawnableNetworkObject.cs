using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.Networking
{
    public class SpawnableNetworkObject : NetworkBehaviour
    {
        [SerializeField] private int spawnObjectId;

        public int SpawnObjectId => spawnObjectId;
    }
}