using System.Collections.Generic;
using _Main.Scripts.Menus;
//using _Main.Scripts.SO.Datas;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Managers
{
    
    public class RoomManager : NetworkBehaviour
    {
        private class RoomData : INetworkSerializable
        {
            public int RoomId;
            public int AvatarId;
            public string Name;
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref RoomId);
                serializer.SerializeValue(ref Name);
                serializer.SerializeValue(ref AvatarId);
            }
        }
        [SerializeField] private List<PlayerAvatarController> playersAvatars;
        private NetworkVariable<int> m_currPlayersInRoom = new NetworkVariable<int>();
        
        
        private Dictionary<ulong, RoomData> m_roomDatas = new Dictionary<ulong, RoomData>();
        private void Awake()
        {
            foreach (var avatar in playersAvatars)
            {
                avatar.DeActivate();
            }
            
            // foreach (var name in playersNamesObj)
            // {
            //     name.gameObject.SetActive(false);
            // }
        }


        // public void OnPlayerJoinRoom(PlayerSpawnData p_spawnData, ulong p_ulong)
        // {
        //     var data = new RoomData();
        //     data.Name = GameManager.Instance.GetPlayersName();
        //     
        //     data.AvatarId = p_spawnData.IconId;
        //     data.RoomId = m_currPlayersInRoom.Value;
        //     
        //     
        //     RequestPlayerJoinRoomUpdateServerRpc(p_ulong, data);
        // }

        [ServerRpc(RequireOwnership = false)]
        private void RequestPlayerJoinRoomUpdateServerRpc(ulong p_ulong, RoomData p_roomData)
        {
            m_roomDatas.Add(p_ulong, p_roomData);
            
            Debug.Log($"Players in room: {m_currPlayersInRoom.Value}");
            m_currPlayersInRoom.Value++;
            RequestPlayerJoinRoomClientRpc(p_ulong, p_roomData);
        }

        [ClientRpc]
        private void RequestPlayerJoinRoomClientRpc(ulong p_ulong, RoomData p_roomData)
        {
            //m_roomDatas.Add(p_ulong, p_roomData);
            foreach (var data in m_roomDatas)
            {
                var roomId = data.Value.RoomId;
                var avatar = playersAvatars[roomId];
                avatar.SetPlayersName(m_roomDatas[data.Key].Name);
                avatar.Activate();
            }

           
        }
    }

}