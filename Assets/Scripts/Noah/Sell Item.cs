using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItem : MonoBehaviour{
    private void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "item"){
            ItemObject droppedItem = other.gameObject.GetComponent<ItemObject>();
            ItemOwnerShip itemOwner = other.gameObject.GetComponent<ItemOwnerShip>();
            if (itemOwner != null){
                ShopManager shopManager = itemOwner.Owner();
                InventoryItemData myData = droppedItem.referenceItem;
                shopManager.playerCredits += myData.sellPrice;
                shopManager.UpdateBalance();
            }
            Destroy(other.gameObject);
        }
    }
}
