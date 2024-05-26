using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class dropItem : MonoBehaviour
{
    [SerializeField]
    List<GameObject> droppedItems;

    Dictionary<InventoryItemData, GameObject> droppedItemsDict;

    public void Start()
    {
    }
    public void playerDropItem()
    {
        return;
    }
}
