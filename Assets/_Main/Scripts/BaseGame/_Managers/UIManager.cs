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


        private bool _toggle;

        private void Start()
        {

            
            MasterManager.Instance.OnChangeLifePoints += UpdateLifeText;

            lifeText.text = MasterManager.Instance.GetLifePoints().ToString();
            
        }


        private void CheckButtonInteractions(int money)
        {
            areaButton.interactable = areaTowerData.Cost <= money;
            rangeButton.interactable = rangeTowerData.Cost <= money;
            rocketButton.interactable = rocketTowerData.Cost <= money;
        }

        public void UpdateLifeText(int x) => lifeText.text = MasterManager.Instance.GetLifePoints().ToString();

        public void UpdateMoneyText(int x)
        {
            moneyText.text = x.ToString();
            CheckButtonInteractions(x);
        }
    }
}