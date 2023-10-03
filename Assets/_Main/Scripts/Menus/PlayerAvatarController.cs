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
        [SerializeField] private List<RawImage> m_images;
        [SerializeField] private TMP_Text name;

        private bool m_isActive;
        private int m_imageId;
        private void Awake()
        {
            foreach (var image in m_images)
            {
                image.gameObject.SetActive(false);
            }
            
            name.gameObject.SetActive(false);
        }


        public void SetImage(int p_imageId)
        {
            m_imageId = p_imageId;
            
        }

        public void SetPlayersName(string p_name)
        {
            name.text = p_name;
        }

        public void Activate()
        {
            name.gameObject.SetActive(true);
            m_images[m_imageId].gameObject.SetActive(true);
            m_isActive = true;
        }
        
        public void DeActivate()
        {
            name.gameObject.SetActive(false);

            foreach (var image in m_images)
            {
                image.gameObject.SetActive(false);
            }
            m_isActive = false;
        }
        public bool GetIsActive() => m_isActive;
    }
}