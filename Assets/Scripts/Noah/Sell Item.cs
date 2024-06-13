using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItem : MonoBehaviour{
    private void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "item"){
            GameObject itemDrop = other.gameObject;
            ActualParent itemParent = itemDrop.GetComponent<ActualParent>();
            if (itemParent != null){
                itemDrop = itemParent.actualParent;
            }
            ItemObject droppedItem = itemDrop.GetComponent<ItemObject>();
            ShopManager shopManager = droppedItem.lastOwner;

            if (shopManager != null){
                InventoryItemDataWrapper myData = droppedItem.referenceItem;
                shopManager.playerCredits += myData.sellPrice;
                shopManager.UpdateBalance();
            }
            SyncWithWorldSpace.Instance.DestoryOnServer(itemDrop);
        }
    }
}
