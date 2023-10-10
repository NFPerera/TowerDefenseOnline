using _Main.Scripts.BaseGame._Managers;
using _Main.Scripts.BaseGame.Models;
using _Main.Scripts.Networking;
using Unity.Netcode;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Controllers
{
    public class ShopController : MonoBehaviour
    {
        
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
