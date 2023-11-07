using _Main.Scripts.BaseGame.ScriptableObjects.Towers;
using _Main.Scripts.Networking;
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

        public ChatManager chat;
        
        //TODO: Cambiar screens por escenas distintas
        [Header("Screens")]
        //[SerializeField] private GameObject gameOverScreen;
        //[SerializeField] private GameObject youWonText;
        //[SerializeField] private GameObject youLoseText;

        private bool _toggle;

        private void Start()
        {
            //gameOverScreen.SetActive(false);
            //youWonText.SetActive(false);
            //youLoseText.SetActive(false);

            
            MasterManager.Instance.OnChangeLifePoints += UpdateLifeText;

            lifeText.text = MasterManager.Instance.GetLifePoints().ToString();
            
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
            //gameOverScreen.SetActive(true);

            if (isWinning)
            {
                //youWonText.SetActive(true);
            }
            else
            {
                
                //youLoseText.SetActive(true);
            }
        }
        
        
        public void UpdateLifeText(int x) => lifeText.text = MasterManager.Instance.GetLifePoints().ToString();

        public void UpdateMoneyText(int x)
        {
            moneyText.text = x.ToString();
            CheckButtonInteractions(x);
        }
    }
}