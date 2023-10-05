using _Main.Scripts.BaseGame.Interfaces;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;

namespace _Main.Scripts.BaseGame.Commands
{
    public class CmdDoDamage : ICommando
    {
        private int m_damage;
        private IDamageable m_healthController;
        
        public CmdDoDamage(IDamageable healthController, int damage)
        {
            m_healthController = healthController;
            m_damage = damage;
        }

        public void Execute() => m_healthController.DoDamage(m_damage);
        public void Undo() => m_healthController.Heal(m_damage);
    }
}