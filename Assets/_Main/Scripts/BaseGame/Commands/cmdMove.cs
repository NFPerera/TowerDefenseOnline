using _Main.Scripts.BaseGame.Clases;
using _Main.Scripts.BaseGame.Interfaces;
using _Main.Scripts.Networking;
using Clases;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Commands
{
    public class CmdMove : ICommando
    {
        private ulong m_ownerId;
        private ulong m_id;
        private Vector3 m_direction;
        private float m_speed;
        
        public CmdMove(ulong p_ownerId, ulong p_objId, Vector3 direction, float p_speed)
        {
            m_ownerId = p_ownerId;
            m_id = p_objId;
            m_direction = direction;
            m_speed = p_speed;
        }

        public void Execute()
        {
            MasterManager.Instance.RequestMoveCommandServerRpc(m_ownerId,m_id, m_direction, m_speed);
            
        }
        public void Undo()
        { 
        }
    }
}