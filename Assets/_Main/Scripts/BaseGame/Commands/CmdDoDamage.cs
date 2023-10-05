using _Main.Scripts.BaseGame.Interfaces;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;
using _Main.Scripts.Networking;

namespace _Main.Scripts.BaseGame.Commands
{
    public class CmdDoDamage : ICommando
    {
        private int m_damage;
        private ulong m_objId;
        
        public CmdDoDamage(ulong objId, int damage)
        {
            m_objId = objId;
            m_damage = damage;
        }

        public void Execute() => MasterManager.Instance.RequestDoDamageServerRpc(m_objId, m_damage);
        

        public void Undo() => MasterManager.Instance.RequestDoHealServerRpc(m_objId, m_damage);
    }
}