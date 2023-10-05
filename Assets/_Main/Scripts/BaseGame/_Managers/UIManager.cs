﻿using _Main.Scripts.BaseGame.ScriptableObjects.Towers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Main.Scripts.BaseGame._Managers
{
    public class UIManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private TowerData areaTowerData;
        [SerializeField] private TowerData rangeTowerData;
        [SerializeField] private TowerData rocketTowerData;

        [Header("Buttons")]
        [SerializeField] private Button areaButton;
        [SerializeField] private Button rangeButton;
        [SerializeField] private Button rocketButton;

        [Header("Texts")] 
        [SerializeField] private TextMeshProUGUI lifeText;
        [SerializeField] private TextMeshProUGUI moneyText;
        
        
        //TODO: Cambiar screens por escenas distintas
        [Header("Screens")]
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private GameObject youWonText;
        [SerializeField] private GameObject youLoseText;

        private bool _toggle;

        private void Start()
        {
            gameOverScreen.SetActive(false);
            youWonText.SetActive(false);
            youLoseText.SetActive(false);
            
            GameManager.Instance.OnChangeLifePoints += SetLifeText;
            GameManager.Instance.OnChangeMoney += SetMoneyText;

            lifeText.text = GameManager.Instance.GetLifePoints().ToString();
            moneyText.text = GameManager.Instance.GetMoney().ToString();
            GameManager.Instance.OnChangeMoney += CheckButtonInteractions;
            
        }


        private void CheckButtonInteractions(int money)
        {
            areaButton.interactable = areaTowerData.Cost <= money;
            rangeButton.interactable = rangeTowerData.Cost <= money;
            rocketButton.interactable = rocketTowerData.Cost <= money;
        }

        public void OnUndoButtonEvent() => GameManager.Instance.SellLastTower();
        public void OnQuitButton() => Application.Quit();
        public void OnRetryButton() => SceneManager.LoadScene("SampleScene");

        public void ActivateGameOverScreen(bool isWinning)
        {
            gameOverScreen.SetActive(true);

            if (isWinning)
            {
                youWonText.SetActive(true);
            }
            else youLoseText.SetActive(true);
        }
        public void OnFastButton()
        {
            if (!_toggle)
            {
                Time.timeScale = 5;
                _toggle = true;
            }
            else
            {
                Time.timeScale = 1;
                _toggle = false;
            }
        }
        
        private void SetLifeText(int x) => lifeText.text = GameManager.Instance.GetLifePoints().ToString();
        private void SetMoneyText(int x) => moneyText.text = GameManager.Instance.GetMoney().ToString();
    }
}