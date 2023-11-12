using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.Networking
{
    public class SpawnableNetworkObject : NetworkBehaviour
    {
        [SerializeField] private int spawnObjectId;

        public int SpawnObjectId => spawnObjectId;
        public ulong MyOwnerId { get; private set; }
        
        [ClientRpc]
        public void SetOwnerIdClientRpc(ulong ownerId)
        {
            MyOwnerId = ownerId;
        }
    }
}