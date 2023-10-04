using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.BaseGame.Models;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Controllers
{
    public class ShopController : MonoBehaviour
    {
        /*
        public void OnButtonDownRangeTower()
        {
            var tower = BuildManager.Instance.GetRangeTowerPrefab();
            
            var towerCost = tower.GetComponent<TowerModel>().GetData().Cost;

            if (towerCost <= GameManager.Instance.GetMoney())
                BuyTower(tower);
        }

        public void OnButtonDownAreaTower()
        {
            var tower = BuildManager.Instance.GetAreaTowerPrefab();
            
            var towerCost = tower.GetComponent<TowerModel>().GetData().Cost;

            if (towerCost <= GameManager.Instance.GetMoney())
                BuyTower(tower);
        }
        
        public void OnButtonDownRocketTower()
        {
            var tower = BuildManager.Instance.GetRocketTowerPrefab();
            
            var towerCost = tower.GetComponent<TowerModel>().GetData().Cost;

            if (towerCost <= GameManager.Instance.GetMoney())
                BuyTower(tower);
        }
        */
        public void OnButtonDownBuyTower(SpawnableNetworkObject p_tower)
        {
            var towerCost = p_tower.GetComponent<TowerModel>().GetData().Cost;

            if (towerCost <= GameManager.Instance.GetMoney())
                BuyTower(p_tower);
        }
        private void BuyTower(SpawnableNetworkObject tower)
        {
            BuildManager.Instance.SetTowerToBuild(tower);
            GameManager.Instance.OnChangeMoney(-tower.GetComponent<TowerModel>().GetData().Cost);
        } 
    }
}
