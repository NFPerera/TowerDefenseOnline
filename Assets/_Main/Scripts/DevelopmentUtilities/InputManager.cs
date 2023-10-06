using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Main.Scripts.DevelopmentUtilities
{
    public class InputManager : MonoBehaviour
    {
        //Todo: hacer refactor de esto
        public static InputManager Instance;

        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private string defaultActionMap;
        
        private string m_lastActionMap;

        private void Awake()
        {
            if (Instance != default)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        private void Initialize()
        {
            ChangeActionMap(defaultActionMap);
        }

        public InputAction GetInputAction(string p_inputId)
        {
            return playerInput.actions[p_inputId];
        }

        public bool TryGetInputAction(string p_inputId, out InputAction p_action)
        {
            p_action = GetInputAction(p_inputId);

            return p_action != default;
        }

        public void SubscribeInput(string p_inputId, Action<InputAction.CallbackContext> p_action)
        {
            if (!TryGetInputAction(p_inputId, out var l_action))
            {
                Debug.LogError("Requested id not found");
                return;
            }

            l_action.performed += p_action;
        }
        
        public void UnsubscribeInput(string p_inputId, Action<InputAction.CallbackContext> p_action)
        {
            if (!TryGetInputAction(p_inputId, out var l_action))
            {
                Debug.LogError("Requested id not found");
                return;
            }

            l_action.performed -= p_action;
        }

        public void ChangeActionMap(string p_actionMap)
        {
            playerInput.SwitchCurrentActionMap(p_actionMap);
        }

        public void RestoredDefaultActionMap()
        {
            ChangeActionMap(defaultActionMap);
        }

        public void SaveLastActionMap()
        {
            m_lastActionMap = playerInput.currentActionMap.name;
        }

        public void RestoresLastActionMap()
        {
            ChangeActionMap(m_lastActionMap);
        }

        public string GetCurrentActionMap() => playerInput.currentActionMap.name;
    }
}