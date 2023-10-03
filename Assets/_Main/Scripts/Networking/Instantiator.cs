
using System.Collections.Generic;
using System.Linq;
// _Main.Scripts.Entities.Player;
using _Main.Scripts.Managers;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.Networking
{
    public class Instantiator : MonoBehaviour
    {
        
        [SerializeField] private List<Transform> spawnPositions;
	    private ulong m_tagId;
        private void Awake()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                //RequestSpawnPlayers();
            }
        }

        // private void RequestSpawnPlayers()
        // {
        //     var players = NetworkManager.Singleton.ConnectedClientsList;
        //     var rnd = Random.Range(0, players.Count);
        //     var tagId = NetworkManager.Singleton.ConnectedClientsList[rnd].ClientId;
        //     
        //     
        //     for (int i = 0; i < players.Count; i++)
        //     {
        //         var currId = players[i].ClientId;
        //     
        //         //hacer aca el Random para saber cual jugador va a ser la mancha
        //         var rndPos = Random.Range(0, spawnPositions.Count);
        //
        //         var p = new ClientRpcParams();
        //         var arr = new ulong[1];
        //         arr[0] = currId;
        //         p.Send.TargetClientIds = arr;
        //         
        //
        //         SpawnPlayerClientRpc(tagId == currId, spawnPositions[rndPos].position, p);
        //         spawnPositions.RemoveAt(rndPos);
        //     }
        // }
        //
        // private void SpawnPlayerClientRpc(bool p_isTag,Vector3 p_spawnPoint, ClientRpcParams p_params)
        // {
        //     var data = GameManager.Instance.GetPlayerSpawnData();
        //     var player = Instantiate<NetworkObject>(data.Prefab);
        //     var id = p_params.Send.TargetClientIds[0];
        //
        //     //ACA setear el transform
        //     player.transform.position = p_spawnPoint;
        //     //Cambiar el network por una instancia local
        //     player.SpawnAsPlayerObject(id);
        //
        //     if (player.TryGetComponent(out PlayerModel model))
        //     {
        //         //TODO: ARREGLAR EL INITIALIZE
        //         //NO SE EJECUTA EN EL LOCAL, SOLO SE EJECUTA EN EL SERVER
        //         model.InitializeClientRpc(p_isTag, p_params);
        //     }
        // }
    }
}
