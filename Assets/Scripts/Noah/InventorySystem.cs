using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySystem : MonoBehaviour
{
    private Dictionary<InventoryItemData, InventoryItem> item_dict;
    public List<InventoryItem> inventory;
    public List<GameObject> droppedItems;

    private void Awake()
    {
        inventory = new List<InventoryItem>();
        item_dict = new Dictionary<InventoryItemData, InventoryItem>();
    }

    public InventoryItem Get(InventoryItemData referenceData) { 
        if (item_dict.TryGetValue(referenceData, out InventoryItem value))
        {
            return value;
        }
        return null;
    }

    public void Add(InventoryItemData referenceData)
    {
        if (item_dict.TryGetValue(referenceData, out InventoryItem value))
        {
            value.AddToStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            item_dict.Add(referenceData, newItem);
        }
    }

    public void Remove(InventoryItemData referenceData)
    {
        if (item_dict.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();
            if (value.stackSize == 0)
            {
                inventory.Remove(value);
                item_dict.Remove(referenceData);
            }
        }
    }
    
    public void DropItem()
    {
        InventoryItem itemToDrop = inventory[0];
        InventoryItemData referenceData = inventory[0].data;
        GameObject itemToSpawn = droppedItems[0];

        if (itemToDrop == null)
        {
            return;
        }
        
        //foreach (GameObject item in droppedItems)
        //{
        //    if (item == referenceData.prefab)
        //    {
        //        itemToSpawn = item;
        //        break;
        //    }
        //}

        GameObject droppedItem = Instantiate(itemToSpawn);
        droppedItem.transform.position = droppedItem.transform.position + new Vector3(0f, 10f, 0f);
    }
}
