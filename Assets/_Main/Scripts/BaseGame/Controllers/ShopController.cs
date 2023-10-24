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
            var myID = NetworkManager.Singleton.LocalClientId; 
            
            MasterManager.Instance.RequestBuyTowerServerRpc(myID, p_tower.SpawnObjectId,towerCost);
        }
        
    }
}
