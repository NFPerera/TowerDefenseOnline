using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Menus
{
    public class PlayerAvatarController : MonoBehaviour
    {
        [SerializeField] private RawImage image;
        [SerializeField] private TMP_Text playersName;

        private bool m_isActive;
        private void Awake()
        {
            
            image.gameObject.SetActive(false);
            
            
            playersName.gameObject.SetActive(false);
        }

        public void SetPlayersName(string p_name)
        {
            playersName.text = p_name;
        }

        public void Activate()
        {
            playersName.gameObject.SetActive(true);
            image.gameObject.SetActive(true);
            m_isActive = true;
        }
        
        public void DeActivate()
        {
            playersName.gameObject.SetActive(false);
            
            image.gameObject.SetActive(false);
            
            m_isActive = false;
        }
        public bool GetIsActive() => m_isActive;
    }
}