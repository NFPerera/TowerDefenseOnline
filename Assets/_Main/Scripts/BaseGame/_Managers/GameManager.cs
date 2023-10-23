using System;
using System.Collections.Generic;
using _Main.Scripts.BaseGame.Controllers;
using _Main.Scripts.BaseGame.Interfaces;
using _Main.Scripts.BaseGame.Interfaces.EnemiesInterfaces;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

namespace _Main.Scripts.BaseGame._Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        //[field: SerializeField] public int MaxLifePoints { get; private set; }
        [field: SerializeField] public List<Transform> PathPoints { get; private set; }
        [SerializeField] private Camera mainCamera;

        private List<ICommando> m_events = new List<ICommando>();
        private Stack<ICommando> m_sellableEvents = new Stack<ICommando>();
        private List<ICommando> m_doneEvents = new List<ICommando>();
        private List<IDamageable> m_enemies = new List<IDamageable>();
        
        

        private const int MaxUndos = 25;
        private string m_playersName;

        private UIManager m_ui;

        
        private void Awake()
        {
            if(Instance != null) Destroy(this);
            Instance = this;

            m_ui = GetComponent<UIManager>();
            
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

            public Camera GetCamera() => mainCamera;

        #endregion
     

        [SerializeField] private Button waveButton;
        private WaveController m_waveController;
        public void ActivateWave()
        {
            MasterManager.Instance.RequestActivateWaveServerRpc();
        }

        public void ToggleWaveButton(bool b)
        {
            waveButton.interactable = b;
        }
        
    }
}