using System;
using System.Collections.Generic;
using _Main.Scripts.BaseGame.Interfaces;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;
using UnityEngine;

namespace _Main.Scripts.BaseGame._Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [field: SerializeField] public int MaxLifePoints { get; private set; }
        [field: SerializeField] public int StartMoneyPoints { get; private set; }
        [field: SerializeField] public List<Transform> PathPoints { get; private set; }
        [SerializeField] private Camera mainCamera;

        private List<ICommando> m_events = new List<ICommando>();
        private Stack<ICommando> m_sellableEvents = new Stack<ICommando>();
        private List<ICommando> m_doneEvents = new List<ICommando>();
        private List<IDamageable> m_enemies = new List<IDamageable>();
        private int m_lifePoints;
        private int m_money;
        
        
        public Action<int> OnChangeLifePoints;
        public Action<int> OnChangeMoney;
        public Action OnClick;
        public Action OnGameOver;

        private const int MaxUndos = 25;


        private UIManager m_ui;

        
        private void Awake()
        {
            if(Instance != null) Destroy(this);
            Instance = this;

            m_ui = GetComponent<UIManager>();
            
            m_lifePoints = MaxLifePoints;
            m_money = StartMoneyPoints;
            
            OnChangeLifePoints += OnLooseLifePoints;
            OnChangeMoney += ChangeMoney;
            OnGameOver += LoseGame;
        }

        
        private void Update()
        {
            foreach (var events in m_events)
            {
                events.Execute();
                m_doneEvents.Add(events);

                if (m_doneEvents.Count > MaxUndos)
                    m_doneEvents.RemoveAt(0);
            }

            m_events.Clear();

            if (Input.GetMouseButtonDown(0))
            {
                OnClick.Invoke();
            }
        }

        #region Facade

            public void AddEventQueue(ICommando commando) => m_events.Add(commando);
            public void AddSellEvent(ICommando commando) => m_sellableEvents.Push(commando);

        #endregion
        
        #region Memento
            public void SellLastTower() => m_sellableEvents.Pop().Undo();
            public void UndoAllEventList()
            {
                if(m_doneEvents.Count <= 0) return;

                for (int i = 0; i < m_doneEvents.Count; i++)
                {
                    m_doneEvents[i].Undo();
                    m_doneEvents.RemoveAt(m_doneEvents.Count - 1);
                }
            }

        #endregion

        #region Getters
            public Stack<ICommando> GetSellableEvents() => m_sellableEvents;

            public int GetLifePoints() => m_lifePoints;
            public int GetMoney() => m_money;
            public Camera GetCamera() => mainCamera;

        #endregion
        
        #region GAME RULES;

            private void OnLooseLifePoints(int lifeChange)
            {
                m_lifePoints -= lifeChange;

                if (m_lifePoints <= 0) LoseGame();
            }

            private void ChangeMoney(int moneyChange)
            {
                m_money += moneyChange;
            }

            private void LoseGame()
            {
                m_ui.ActivateGameOverScreen(false);
            }

        #endregion
        
        
        
    }
}