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
            //COMO SE QUIEN ESTA APRETANDO EL BOTON??????
            var myMoney = MasterManager.Instance.GetPlayersCurrMoney(NetworkManager.Singleton.LocalClientId);
            if (towerCost <= myMoney)
                BuyTower(p_tower);
        }
        private void BuyTower(SpawnableNetworkObject tower)
        {
            var towerCost = tower.GetComponent<TowerModel>().GetData().Cost;
            BuildManager.Instance.SetTowerToBuild(tower);
            MasterManager.Instance.RequestChangeMoneyServerRpc(NetworkManager.Singleton.LocalClientId, towerCost);
        } 
    }
}
