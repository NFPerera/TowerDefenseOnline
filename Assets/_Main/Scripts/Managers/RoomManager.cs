using System.Collections.Generic;
using _Main.Scripts.Menus;
//using _Main.Scripts.SO.Datas;
using TMPro;
using Unity.Netcode;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Managers
{
    
    public class RoomManager : NetworkBehaviour
    {
        private class RoomData : INetworkSerializable
        {
            public int RoomId;
            public string Name;
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref RoomId);
                serializer.SerializeValue(ref Name);
            }
        }
        
        
        [SerializeField] private List<PlayerAvatarController> playersAvatars;
        //private NetworkVariable<int> m_currPlayersInRoom = new NetworkVariable<int>();
        private Dictionary<ulong, RoomData> m_roomDatas = new Dictionary<ulong, RoomData>();
        private int m_playersCount;
        private void Awake()
        {
            if (!IsServer)
            {
                this.enabled = false;
                return;
            }
            foreach (var avatar in playersAvatars)
            {
                avatar.DeActivate();
            }

            m_playersCount = 0;
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
        public void RequestPlayerJoinRoomUpdateServerRpc(ulong p_ulong, string p_playersName)
        {

            var data = new RoomData();
            data.Name = p_playersName;
            data.RoomId = m_playersCount;
            m_roomDatas.Add(p_ulong, data);
            
            Debug.Log($"Players in room: {m_playersCount}");
            m_playersCount++;
            RefreshWaitingRoomView();
        }

        private void RefreshWaitingRoomView()
        {
            foreach (var data in m_roomDatas)
            {
                var roomId = data.Value.RoomId;
                var avatar = playersAvatars[roomId];
                avatar.SetPlayersName(data.Value.Name);
                avatar.Activate();
            }

           
        }
    }

}