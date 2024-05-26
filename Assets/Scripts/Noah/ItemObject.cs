using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;
    InventorySystem system = new InventorySystem();

    public void pickUpItem()
    {
        system.Add(referenceItem);
        Destroy(gameObject);
    }
}
