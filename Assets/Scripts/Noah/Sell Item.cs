using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItem : MonoBehaviour
{
    // reference to access player credits
    [SerializeField]
    private ShopManager shopManager;

    private void OnTriggerEnter(Collider other)
    {
        //make sure we're actually colliding with an item
        if (other.gameObject.tag == "item")
        {
            //get the item's scriptable object
            ItemObject droppedItem = other.gameObject.GetComponent<ItemObject>();
            InventoryItemData myData = droppedItem.referenceItem;

            //add on to the player's balance and destroy dropped item
            shopManager.playerCredits += myData.sellPrice;
            shopManager.playerBalanceText.text = "Current Balance: " + shopManager.playerCredits;

            Destroy(other.gameObject);
        }
    }
}
