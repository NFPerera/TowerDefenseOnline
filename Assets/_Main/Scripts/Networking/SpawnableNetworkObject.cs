using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.Networking
{
    public class SpawnableNetworkObject : NetworkBehaviour
    {
        [SerializeField] private int spawnObjectId;

        public int SpawnObjectId => spawnObjectId;
        public ulong OwnerId { get; private set; }
        public void SetOwnerId(ulong p_ownerId) => OwnerId = p_ownerId;
    }
}