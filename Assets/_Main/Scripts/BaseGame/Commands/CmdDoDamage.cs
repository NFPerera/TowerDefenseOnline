using _Main.Scripts.BaseGame.Interfaces;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;
using _Main.Scripts.Networking;

namespace _Main.Scripts.BaseGame.Commands
{
    public class CmdDoDamage : ICommando
    {
        private int m_damage;
        private ulong m_objId;
        private ulong m_attackerId;
        
        public CmdDoDamage(ulong objId, ulong attackerId,int damage)
        {
            m_objId = objId;
            m_damage = damage;
            m_attackerId = attackerId;
        }

        public void Execute() => MasterManager.Instance.RequestDoDamageServerRpc(m_objId,m_attackerId, m_damage);
        

        public void Undo() => MasterManager.Instance.RequestDoHealServerRpc(m_objId, m_damage);
    }
}